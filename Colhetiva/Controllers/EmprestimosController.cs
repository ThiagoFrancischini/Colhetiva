using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using System.Collections.Generic;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmprestimosController : Controller
{
    private readonly ColhetivaDbContext _db;

    public EmprestimosController(ColhetivaDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Solicitar(Guid ferramentaId, Guid hortaId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para solicitar empréstimo.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var ferramenta = await _db.Ferramentas
            .Include(f => f.Horta)
            .FirstOrDefaultAsync(f => f.Id == ferramentaId);

        if (ferramenta == null)
        {
            TempData["MensagemErro"] = "Ferramenta não encontrada.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        if (ferramenta.Status != StatusFerramenta.Disponivel)
        {
            TempData["MensagemErro"] = "Ferramenta não está disponível para empréstimo.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        // já existe pedido pendente ou empréstimo ativo para este usuário e ferramenta?
        var existe = await _db.Emprestimos.AnyAsync(e =>
            e.UsuarioId == usuarioId &&
            e.FerramentaId == ferramentaId &&
            (e.Status == StatusEmprestimo.Pendente || (e.Status == StatusEmprestimo.Aprovado && e.DataDevolucao == null)));

        if (existe)
        {
            TempData["MensagemInfo"] = "Você já possui um pedido/ empréstimo ativo para esta ferramenta.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
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

        TempData["MensagemSucesso"] = "Pedido de empréstimo enviado. Aguarde a aprovação do responsável.";
        return RedirectToAction("Details", "Horta", new { id = hortaId });
    }
    /*
    [HttpGet]
    public async Task<IActionResult> Manage()
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para acessar pedidos de empréstimo.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var pedidos = await _db.Emprestimos
            .Include(e => e.Ferramenta)
                .ThenInclude(f => f.Horta)
            .Include(e => e.Usuario)
            .Where(e => e.Ferramenta.Horta.UsuarioId == usuarioId && e.Status == StatusEmprestimo.Pendente)
            .OrderBy(e => e.DataRetirada)
            .ToListAsync();

        return View(pedidos);
    }
    */
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Aprovar(Guid emprestimoId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para aprovar pedidos.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var e = await _db.Emprestimos
            .Include(x => x.Ferramenta)
                .ThenInclude(f => f.Horta)
            .FirstOrDefaultAsync(x => x.Id == emprestimoId);

        if (e == null) return NotFound();
        if (e.Ferramenta.Horta.UsuarioId != usuarioId) return Forbid();
        if (e.Status != StatusEmprestimo.Pendente)
        {
            TempData["MensagemInfo"] = "Pedido já processado.";
            return RedirectToAction("Manage");
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
        return RedirectToAction("Manage");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reprovar(Guid emprestimoId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para reprovar pedidos.";
            return RedirectToAction("Login", "Account");
        }
        var usuarioId = Guid.Parse(usuarioIdStr);

        var e = await _db.Emprestimos
            .Include(x => x.Ferramenta)
                .ThenInclude(f => f.Horta)
            .FirstOrDefaultAsync(x => x.Id == emprestimoId);

        if (e == null) return NotFound();
        if (e.Ferramenta.Horta.UsuarioId != usuarioId) return Forbid();
        if (e.Status != StatusEmprestimo.Pendente)
        {
            TempData["MensagemInfo"] = "Pedido já processado.";
            return RedirectToAction("Manage");
        }

        e.Status = StatusEmprestimo.Recusado;
        await _db.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Pedido reprovado.";
        return RedirectToAction("Manage");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Devolver(Guid emprestimoId, Guid hortaId)
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para devolver ferramenta.";
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
            TempData["MensagemInfo"] = "Empréstimo não pode ser devolvido.";
            return RedirectToAction("Details", "Horta", new { id = hortaId });
        }

        e.DataDevolucao = DateTime.UtcNow;
        e.Ferramenta.Status = StatusFerramenta.Disponivel;
        await _db.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Ferramenta devolvida com sucesso.";
        return RedirectToAction("Details", "Horta", new { id = hortaId });
    }

    [HttpGet]
    public async Task<IActionResult> Meus()
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            TempData["MensagemInfo"] = "Faça login para ver seus empréstimos.";
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
