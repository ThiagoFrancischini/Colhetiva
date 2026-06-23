using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Colhetiva.Controllers
{
    public class SolicitacoesController : Controller
    {
        private readonly ColhetivaDbContext _db;

        public SolicitacoesController(ColhetivaDbContext db)
        {
            _db = db;
        }

        // GET: /Solicitacoes/Index?hortaId=...
        [HttpGet]
        public async Task<IActionResult> Index(Guid hortaId)
        {
            if (hortaId == Guid.Empty)
                return RedirectToAction("Index", "Home");

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para solicitar um canteiro.";
                return RedirectToAction("Login", "Account");
            }

            var usuarioId = Guid.Parse(usuarioIdStr);

            var horta = await _db.Hortas
                .Include(h => h.Endereco)
                    .ThenInclude(e => e.Cidade)
                        .ThenInclude(c => c.Estado)
                .Include(h => h.Usuario)
                .Include(h => h.Canteiros)
                .FirstOrDefaultAsync(h => h.Id == hortaId);

            if (horta == null)
                return NotFound();

            var pendentes = await _db.Solicitacoes
                .Where(s => s.UsuarioId == usuarioId && s.Status == StatusSolicitacao.Pendente)
                .Select(s => s.CanteiroId)
                .ToListAsync();

            ViewBag.PendingCanteiros = pendentes;
            return View(horta);
        }

        // POST: /Solicitacoes/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(Guid canteiroId, Guid hortaId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para solicitar um canteiro.";
                return RedirectToAction("Login", "Account");
            }

            var usuarioId = Guid.Parse(usuarioIdStr);

            var canteiro = await _db.Canteiros
                .FirstOrDefaultAsync(c => c.Id == canteiroId);

            if (canteiro == null)
            {
                TempData["MensagemErro"] = "Canteiro não encontrado.";
                return RedirectToAction("Index", new { hortaId });
            }

            if (canteiro.Status != StatusCanteiro.Disponivel)
            {
                TempData["MensagemErro"] = "Canteiro não está disponível para solicitação.";
                return RedirectToAction("Index", new { hortaId });
            }

            var existePendente = await _db.Solicitacoes.AnyAsync(s =>
                s.UsuarioId == usuarioId &&
                s.CanteiroId == canteiroId &&
                s.Status == StatusSolicitacao.Pendente);

            if (existePendente)
            {
                TempData["MensagemInfo"] = "Você já possui uma solicitação pendente para esse canteiro.";
                return RedirectToAction("Index", new { hortaId });
            }

            var solicitacao = new Solicitacao
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                CanteiroId = canteiroId,
                Status = StatusSolicitacao.Pendente
            };

            await _db.Solicitacoes.AddAsync(solicitacao);
            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Solicitação enviada com sucesso. Aguarde a resposta do gestor.";
            return RedirectToAction("Index", new { hortaId });
        }

        // --- Painel do gestor / Manage e outras ações já existentes ---
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para acessar as solicitações.";
                return RedirectToAction("Login", "Account");
            }

            var usuarioId = Guid.Parse(usuarioIdStr);

            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            var hortasQuery = _db.Hortas.AsNoTracking().Where(h =>
                h.UsuarioId == usuarioId ||
                (usuario != null && usuario.OrganizationId.HasValue && h.OrganizationId.HasValue && h.OrganizationId == usuario.OrganizationId));

            var hortaIds = await hortasQuery.Select(h => h.Id).ToListAsync();

            var solicitacoes = await _db.Solicitacoes
                .Include(s => s.Canteiro)
                    .ThenInclude(c => c.Horta)
                .Include(s => s.Usuario)
                .Where(s => s.Status == StatusSolicitacao.Pendente && hortaIds.Contains(s.Canteiro.HortaId))
                .OrderBy(s => s.DataPedido)
                .ToListAsync();

            var emprestimos = await _db.Emprestimos
                .Include(e => e.Ferramenta)
                    .ThenInclude(f => f.Horta)
                .Include(e => e.Usuario)
                .Where(e => e.Status == StatusEmprestimo.Pendente && hortaIds.Contains(e.Ferramenta.HortaId))
                .OrderBy(e => e.DataRetirada)
                .ToListAsync();

            var participantes = await _db.Solicitacoes
                .Include(s => s.Canteiro)
                    .ThenInclude(c => c.Horta)
                .Include(s => s.Usuario)
                .Where(s => s.Status == StatusSolicitacao.Aprovado && hortaIds.Contains(s.Canteiro.HortaId))
                .OrderBy(s => s.DataPedido)
                .ToListAsync();

            var totalParticipants = participantes.Count;
            var totalCanteiros = await _db.Canteiros.Where(c => hortaIds.Contains(c.HortaId)).CountAsync();
            var occupiedCanteiros = await _db.Canteiros.Where(c => hortaIds.Contains(c.HortaId) && c.Status == StatusCanteiro.Ocupado).CountAsync();
            var taxaOcupacao = totalCanteiros == 0 ? 0m : (decimal)occupiedCanteiros * 100m / totalCanteiros;
            var ferramentasEmUso = await _db.Ferramentas.Where(f => hortaIds.Contains(f.HortaId) && f.Status == StatusFerramenta.EmUso).CountAsync();

            var inventario = await _db.Ferramentas
                .Include(f => f.Horta)
                .Where(f => hortaIds.Contains(f.HortaId))
                .OrderBy(f => f.Nome)
                .ToListAsync();

            ViewBag.PendingEmprestimos = emprestimos;
            ViewBag.Participantes = participantes;
            ViewBag.TotalParticipants = totalParticipants;
            ViewBag.TaxaOcupacao = Math.Round(taxaOcupacao, 1);
            ViewBag.FerramentasEmUso = ferramentasEmUso;
            ViewBag.Inventario = inventario;

            return View(solicitacoes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFerramenta(Guid ferramentaId, string nome, string status)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para editar ferramentas.";
                return RedirectToAction("Login", "Account");
            }
            var usuarioId = Guid.Parse(usuarioIdStr);

            var ferramenta = await _db.Ferramentas
                .Include(f => f.Horta)
                .FirstOrDefaultAsync(f => f.Id == ferramentaId);

            if (ferramenta == null) return NotFound();

            var usuario = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);
            var pertenceOrg = usuario != null && usuario.OrganizationId.HasValue && ferramenta.Horta.OrganizationId.HasValue && usuario.OrganizationId == ferramenta.Horta.OrganizationId;
            if (ferramenta.Horta.UsuarioId != usuarioId && !pertenceOrg) return Forbid();

            ferramenta.Nome = (nome ?? string.Empty).Trim();

            if (Enum.TryParse<StatusFerramenta>(status, ignoreCase: true, out var st))
            {
                ferramenta.Status = st;
            }

            await _db.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Ferramenta atualizada.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFerramentaActive(Guid ferramentaId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para alterar ferramentas.";
                return RedirectToAction("Login", "Account");
            }
            var usuarioId = Guid.Parse(usuarioIdStr);

            var ferramenta = await _db.Ferramentas
                .Include(f => f.Horta)
                .FirstOrDefaultAsync(f => f.Id == ferramentaId);

            if (ferramenta == null) return NotFound();

            var usuario = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);
            var pertenceOrg = usuario != null && usuario.OrganizationId.HasValue && ferramenta.Horta.OrganizationId.HasValue && usuario.OrganizationId == ferramenta.Horta.OrganizationId;
            if (ferramenta.Horta.UsuarioId != usuarioId && !pertenceOrg) return Forbid();

            if (ferramenta.Status == StatusFerramenta.Inativa)
                ferramenta.Status = StatusFerramenta.Disponivel;
            else
                ferramenta.Status = StatusFerramenta.Inativa;

            await _db.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Status da ferramenta atualizado.";
            return RedirectToAction("Manage");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprovar(Guid solicitacaoId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para aprovar solicitações.";
                return RedirectToAction("Login", "Account");
            }
            var usuarioId = Guid.Parse(usuarioIdStr);

            var usuario = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();
            var horta = s.Canteiro?.Horta;
            if (horta == null) return NotFound();

            var pertenceOrg = usuario != null && usuario.OrganizationId.HasValue && horta.OrganizationId.HasValue && usuario.OrganizationId == horta.OrganizationId;
            if (horta.UsuarioId != usuarioId && !pertenceOrg) return Forbid();

            if (s.Status != StatusSolicitacao.Pendente)
            {
                TempData["MensagemInfo"] = "Solicitação já processada.";
                return RedirectToAction("Manage");
            }

            s.Status = StatusSolicitacao.Aprovado;
            s.Canteiro.Status = StatusCanteiro.Ocupado;

            var outros = await _db.Solicitacoes
                .Where(x => x.CanteiroId == s.CanteiroId && x.Status == StatusSolicitacao.Pendente && x.Id != s.Id)
                .ToListAsync();
            foreach (var o in outros) o.Status = StatusSolicitacao.Recusado;

            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Solicitação aprovada. Participante confirmado.";
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reprovar(Guid solicitacaoId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para reprovar solicitações.";
                return RedirectToAction("Login", "Account");
            }
            var usuarioId = Guid.Parse(usuarioIdStr);

            var usuario = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();
            var horta = s.Canteiro?.Horta;
            if (horta == null) return NotFound();

            var pertenceOrg = usuario != null && usuario.OrganizationId.HasValue && horta.OrganizationId.HasValue && usuario.OrganizationId == horta.OrganizationId;
            if (horta.UsuarioId != usuarioId && !pertenceOrg) return Forbid();

            if (s.Status != StatusSolicitacao.Pendente)
            {
                TempData["MensagemInfo"] = "Solicitação já processada.";
                return RedirectToAction("Manage");
            }

            s.Status = StatusSolicitacao.Recusado;
            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Solicitação recusada.";
            return RedirectToAction("Manage");
        }
    }
    
}