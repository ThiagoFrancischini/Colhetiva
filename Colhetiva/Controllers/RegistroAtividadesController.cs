using Colhetiva.Core.Entities;
using Colhetiva.DTOs;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Mappings;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.Services;
using Colhetiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Controllers;

public class RegistroAtividadesController : Controller
{
    private static readonly string[] AtividadesSugeridas =
    {
        "Rega", "Poda", "Adubação", "Colheita", "Limpeza", "Manutenção de Cerca"
    };

    private readonly ColhetivaDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IRegistroAtividadeService _registroAtividadeService;

    public RegistroAtividadesController(
        ColhetivaDbContext db,
        ICurrentUserService currentUser,
        IRegistroAtividadeService registroAtividadeService)
    {
        _db = db;
        _currentUser = currentUser;
        _registroAtividadeService = registroAtividadeService;
    }

    // GET: /RegistroAtividades/Diario?ano=&mes=
    [HttpGet]
    public async Task<IActionResult> Diario(int? ano, int? mes)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para ver seu diário de cultivo.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var hoje = DateTime.Now;
        var anoAtual = ano ?? hoje.Year;
        var mesAtual = mes ?? hoje.Month;
        if (mesAtual < 1) { mesAtual = 12; anoAtual--; }
        if (mesAtual > 12) { mesAtual = 1; anoAtual++; }

        var registros = await _registroAtividadeService.GetDiarioPessoalAsync(usuarioId, anoAtual, mesAtual);

        var viewModel = new CalendarioMesViewModel
        {
            Ano = anoAtual,
            Mes = mesAtual,
            Semanas = MontarSemanas(anoAtual, mesAtual)
        };

        foreach (var registro in registros)
        {
            var dia = registro.DataHora.ToLocalTime().Date;
            if (!viewModel.AtividadesPorDia.TryGetValue(dia, out var lista))
            {
                lista = new List<ItemCalendarioDto>();
                viewModel.AtividadesPorDia[dia] = lista;
            }
            lista.Add(registro.ToItemCalendarioDto());
        }

        return View(viewModel);
    }

    // GET: /RegistroAtividades/Timeline?hortaId=
    [HttpGet]
    public async Task<IActionResult> Timeline(Guid hortaId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para ver a timeline da horta.";
            return RedirectToAction("Login", "Account");
        }

        var horta = await _db.Hortas.FirstOrDefaultAsync(h => h.Id == hortaId);
        if (horta == null) return NotFound();

        if (!await PodeAcessarHortaAsync(horta))
        {
            TempData["MensagemInfo"] = "Você precisa participar desta horta para ver a timeline.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        var registros = await _registroAtividadeService.GetTimelineHortaAsync(hortaId);

        ViewBag.Horta = horta;
        return View(registros.Select(r => r.ToTimelineHortaDto()).ToList());
    }

    // GET: /RegistroAtividades/Registrar?hortaId=&canteiroId=
    [HttpGet]
    public async Task<IActionResult> Registrar(Guid hortaId, Guid? canteiroId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para registrar uma atividade.";
            return RedirectToAction("Login", "Account");
        }

        var horta = await _db.Hortas
            .Include(h => h.Canteiros)
            .FirstOrDefaultAsync(h => h.Id == hortaId);
        if (horta == null) return NotFound();

        if (!await PodeAcessarHortaAsync(horta))
        {
            TempData["MensagemInfo"] = "Você precisa participar desta horta para registrar atividades.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        var viewModel = new RegistrarAtividadeViewModel
        {
            Horta = horta,
            CanteiroIdPreSelecionado = canteiroId,
            AtividadesSugeridas = AtividadesSugeridas.ToList()
        };

        return View(viewModel);
    }

    // POST: /RegistroAtividades/Registrar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(
        Guid hortaId,
        Guid? canteiroId,
        DateTime dataHora,
        int tzOffsetMinutes,
        string atividadeSelecionada,
        string? atividadeOutro,
        string? observacoes,
        string? fotoUrl)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para registrar uma atividade.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var horta = await _db.Hortas.FirstOrDefaultAsync(h => h.Id == hortaId);
        if (horta == null) return NotFound();

        if (!await PodeAcessarHortaAsync(horta))
        {
            TempData["MensagemInfo"] = "Você precisa participar desta horta para registrar atividades.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        if (canteiroId.HasValue)
        {
            var canteiroValido = await _db.Canteiros.AnyAsync(c => c.Id == canteiroId.Value && c.HortaId == hortaId);
            if (!canteiroValido)
            {
                TempData["MensagemErro"] = "Canteiro inválido para esta horta.";
                return RedirectToAction("Registrar", new { hortaId, canteiroId });
            }
        }

        var atividade = (atividadeSelecionada == "Outro" ? atividadeOutro : atividadeSelecionada)?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(atividade) || atividade.Length > 100)
        {
            TempData["MensagemErro"] = "Informe uma atividade válida (até 100 caracteres).";
            return RedirectToAction("Registrar", new { hortaId, canteiroId });
        }

        if (!string.IsNullOrWhiteSpace(observacoes) && observacoes.Length > 500)
        {
            TempData["MensagemErro"] = "Observações devem ter no máximo 500 caracteres.";
            return RedirectToAction("Registrar", new { hortaId, canteiroId });
        }

        if (!string.IsNullOrWhiteSpace(fotoUrl))
        {
            if (fotoUrl.Length > 2083 ||
                !Uri.TryCreate(fotoUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                TempData["MensagemErro"] = "Informe uma URL de imagem válida (http ou https).";
                return RedirectToAction("Registrar", new { hortaId, canteiroId });
            }
        }

        var dataHoraUtc = DateTime.SpecifyKind(dataHora, DateTimeKind.Unspecified).AddMinutes(tzOffsetMinutes);
        dataHoraUtc = DateTime.SpecifyKind(dataHoraUtc, DateTimeKind.Utc);

        var dto = new CriarAtividadeDto(hortaId, canteiroId, dataHoraUtc, atividade, observacoes?.Trim(), fotoUrl?.Trim());
        var entidade = dto.ToEntity(usuarioId);

        await _registroAtividadeService.CriarAtividadeAsync(entidade);

        TempData["MensagemSucesso"] = "Atividade registrada com sucesso.";
        return RedirectToAction("Timeline", new { hortaId });
    }

    private async Task<bool> PodeAcessarHortaAsync(Horta horta)
    {
        return await _currentUser.CanManageHortaAsync(horta) ||
               await _currentUser.IsParticipantOfHortaAsync(horta.Id);
    }

    private static List<List<DateTime?>> MontarSemanas(int ano, int mes)
    {
        var primeiroDia = new DateTime(ano, mes, 1);
        var ultimoDia = primeiroDia.AddMonths(1).AddDays(-1);

        var inicioGrade = primeiroDia.AddDays(-(int)primeiroDia.DayOfWeek);
        var fimGrade = ultimoDia.AddDays(6 - (int)ultimoDia.DayOfWeek);

        var semanas = new List<List<DateTime?>>();
        var semanaAtual = new List<DateTime?>();

        for (var dia = inicioGrade; dia <= fimGrade; dia = dia.AddDays(1))
        {
            semanaAtual.Add(dia.Month == mes ? dia : null);
            if (semanaAtual.Count == 7)
            {
                semanas.Add(semanaAtual);
                semanaAtual = new List<DateTime?>();
            }
        }

        return semanas;
    }
}
