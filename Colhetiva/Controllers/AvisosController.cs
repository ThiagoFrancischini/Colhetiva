using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Controllers;

public class AvisosController : Controller
{
    private readonly ColhetivaDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IAvisoService _avisoService;

    public AvisosController(ColhetivaDbContext db, ICurrentUserService currentUser, IAvisoService avisoService)
    {
        _db = db;
        _currentUser = currentUser;
        _avisoService = avisoService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publicar(Guid hortaId, string titulo, string conteudo)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para publicar um aviso.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var horta = await _db.Hortas.FirstOrDefaultAsync(h => h.Id == hortaId);
        if (horta == null)
            return NotFound();

        if (!await _currentUser.CanManageHortaAsync(horta))
        {
            TempData["MensagemInfo"] = "Você não tem permissão para publicar avisos nesta horta.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        titulo = titulo?.Trim() ?? string.Empty;
        conteudo = conteudo?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(titulo) || titulo.Length > 150)
        {
            TempData["MensagemErro"] = "Informe um título válido (até 150 caracteres).";
            return RedirectToAction("Manage", "Horta", new { id = hortaId });
        }

        if (string.IsNullOrWhiteSpace(conteudo) || conteudo.Length > 2000)
        {
            TempData["MensagemErro"] = "Informe um conteúdo válido (até 2000 caracteres).";
            return RedirectToAction("Manage", "Horta", new { id = hortaId });
        }

        var aviso = new Aviso
        {
            HortaId = hortaId,
            UsuarioId = usuarioId,
            Titulo = titulo,
            Conteudo = conteudo
        };

        await _avisoService.PublicarAvisoAsync(aviso);

        TempData["MensagemSucesso"] = "Aviso publicado e participantes notificados.";
        return RedirectToAction("Manage", "Horta", new { id = hortaId });
    }
}
