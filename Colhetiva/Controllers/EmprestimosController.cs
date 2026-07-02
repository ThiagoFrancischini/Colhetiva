using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Services;
using System.Collections.Generic;

namespace Colhetiva.Controllers;

public class EmprestimosController : Controller
{
    private readonly ColhetivaDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EmprestimosController(ColhetivaDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid hortaId)
    {
        if (hortaId == Guid.Empty)
            return RedirectToAction("Index", "Home");

        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Fa�a login para solicitar empr�stimo.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        if (await _currentUser.IsOrganizationAdminAsync())
        {
            TempData["MensagemInfo"] = "Usuários de organização não podem solicitar empréstimo de ferramentas.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        var horta = await _db.Hortas
            .Include(h => h.Endereco)
                .ThenInclude(e => e.Cidade)
                    .ThenInclude(c => c.Estado)
            .Include(h => h.Usuario)
            .Include(h => h.Ferramentas)
            .FirstOrDefaultAsync(h => h.Id == hortaId);

        if (horta == null) return NotFound();

        var pendentes = await _db.Emprestimos
            .Where(e => e.UsuarioId == usuarioId && e.Status == StatusEmprestimo.Pendente)
            .Select(e => e.FerramentaId)
            .ToListAsync();

        var ativos = await _db.Emprestimos
            .Where(e => e.UsuarioId == usuarioId && e.Status == StatusEmprestimo.Aprovado && e.DataDevolucao == null)
            .Select(e => e.FerramentaId)
            .ToListAsync();

        ViewBag.PendingFerramentas = pendentes;
        ViewBag.ActiveFerramentas = ativos;

        return View(horta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Solicitar(Guid ferramentaId, Guid hortaId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Fa�a login para solicitar empr�stimo.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        if (await _currentUser.IsOrganizationAdminAsync())
        {
            TempData["MensagemInfo"] = "Usuários de organização não podem solicitar empréstimo de ferramentas.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        var ferramenta = await _db.Ferramentas
            .Include(f => f.Horta)
            .FirstOrDefaultAsync(f => f.Id == ferramentaId);

        if (ferramenta == null)
        {
            TempData["MensagemErro"] = "Ferramenta n�o encontrada.";
            return RedirectToAction("Index", new { hortaId });
        }

        if (ferramenta.Status != StatusFerramenta.Disponivel)
        {
            TempData["MensagemErro"] = "Ferramenta n�o est� dispon�vel para empr�stimo.";
            return RedirectToAction("Index", new { hortaId });
        }

        var existe = await _db.Emprestimos.AnyAsync(e =>
            e.UsuarioId == usuarioId &&
            e.FerramentaId == ferramentaId &&
            (e.Status == StatusEmprestimo.Pendente || (e.Status == StatusEmprestimo.Aprovado && e.DataDevolucao == null)));

        if (existe)
        {
            TempData["MensagemInfo"] = "Voc� j� possui um pedido/ empr�stimo ativo para esta ferramenta.";
            return RedirectToAction("Index", new { hortaId });
        }

        var emprestimo = new Emprestimo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            FerramentaId = ferramentaId,
            Status = StatusEmprestimo.Pendente,
            DataRetirada = DateTime.UtcNow
        };

        await _db.Emprestimos.AddAsync(emprestimo);
        await _db.SaveChangesAsync();

        // Pedido salvo com Status = Pendente � gestor ver� imediatamente em Manage
        TempData["MensagemSucesso"] = "Pedido de empr�stimo enviado. Aguarde a aprova��o do respons�vel.";
        return RedirectToAction("Index", new { hortaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Aprovar(Guid emprestimoId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Fa�a login para aprovar pedidos.";
            return RedirectToAction("Login", "Account");
        }

        var e = await _db.Emprestimos
            .Include(x => x.Ferramenta)
                .ThenInclude(f => f.Horta)
            .FirstOrDefaultAsync(x => x.Id == emprestimoId);

        if (e == null) return NotFound();
        var horta = e.Ferramenta?.Horta;
        if (horta == null) return NotFound();
        if (!await _currentUser.CanManageHortaAsync(horta)) return Forbid();

        if (e.Status != StatusEmprestimo.Pendente)
        {
            TempData["MensagemInfo"] = "Pedido j� processado.";
            return RedirectToAction("Details", "Horta", new { id = horta.Id });
        }

        e.Status = StatusEmprestimo.Aprovado;
        e.DataRetirada = DateTime.UtcNow;
        e.Ferramenta.Status = StatusFerramenta.EmUso;

        var outros = await _db.Emprestimos
            .Where(x => x.FerramentaId == e.FerramentaId && x.Status == StatusEmprestimo.Pendente && x.Id != e.Id)
            .ToListAsync();
        foreach (var o in outros) o.Status = StatusEmprestimo.Recusado;

        await _db.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Pedido aprovado. Ferramenta marcada como em uso.";
        return RedirectToAction("Details", "Horta", new { id = horta.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reprovar(Guid emprestimoId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Fa�a login para reprovar pedidos.";
            return RedirectToAction("Login", "Account");
        }

        var e = await _db.Emprestimos
            .Include(x => x.Ferramenta)
                .ThenInclude(f => f.Horta)
            .FirstOrDefaultAsync(x => x.Id == emprestimoId);

        if (e == null) return NotFound();
        var horta = e.Ferramenta?.Horta;
        if (horta == null) return NotFound();
        if (!await _currentUser.CanManageHortaAsync(horta)) return Forbid();

        if (e.Status != StatusEmprestimo.Pendente)
        {
            TempData["MensagemInfo"] = "Pedido j� processado.";
            return RedirectToAction("Details", "Horta", new { id = horta.Id });
        }

        e.Status = StatusEmprestimo.Recusado;
        await _db.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Pedido reprovado.";
        return RedirectToAction("Details", "Horta", new { id = horta.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Devolver(Guid emprestimoId, Guid hortaId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Fa�a login para devolver ferramenta.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var e = await _db.Emprestimos
            .Include(x => x.Ferramenta)
            .FirstOrDefaultAsync(x => x.Id == emprestimoId);

        if (e == null) return NotFound();
        if (e.UsuarioId != usuarioId) return Forbid();
        if (e.Status != StatusEmprestimo.Aprovado || e.DataDevolucao != null)
        {
            TempData["MensagemInfo"] = "Empr�stimo n�o pode ser devolvido.";
            return RedirectToAction("Index", new { hortaId });
        }

        e.DataDevolucao = DateTime.UtcNow;
        e.Ferramenta.Status = StatusFerramenta.Disponivel;
        await _db.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Ferramenta devolvida com sucesso.";
        return RedirectToAction("Index", new { hortaId });
    }

    [HttpGet]
    public async Task<IActionResult> Meus()
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Fa�a login para ver seus empr�stimos.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var meus = await _db.Emprestimos
            .Include(e => e.Ferramenta)
                .ThenInclude(f => f.Horta)
            .Where(e => e.UsuarioId == usuarioId)
            .OrderByDescending(e => e.DataRetirada)
            .ToListAsync();

        return View(meus);
    }
}