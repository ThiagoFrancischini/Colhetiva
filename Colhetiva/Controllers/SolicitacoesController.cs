using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Services;
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
        private readonly ICurrentUserService _currentUser;

        public SolicitacoesController(ColhetivaDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
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

            if (await _currentUser.IsOrganizationAdminAsync())
            {
                TempData["MensagemInfo"] = "Usuários de organização não podem solicitar participação em canteiros.";
                return RedirectToAction("Details", "Horta", new { id = hortaId });
            }

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

            if (await _currentUser.IsOrganizationAdminAsync())
            {
                TempData["MensagemInfo"] = "Usuários de organização não podem solicitar participação em canteiros.";
                return RedirectToAction("Details", "Horta", new { id = hortaId });
            }

            var canteiro = await _db.Canteiros
                .FirstOrDefaultAsync(c => c.Id == canteiroId);

            if (canteiro == null)
            {
                TempData["MensagemErro"] = "Canteiro não encontrado.";
                return RedirectToAction("Index", new { hortaId });
            }

            if (canteiro.Status != StatusCanteiro.Disponivel)
            {
                TempData["MensagemErro"] = "Canteiro não está disponível para Solicitação.";
                return RedirectToAction("Index", new { hortaId });
            }

            var existePendente = await _db.Solicitacoes.AnyAsync(s =>
                s.UsuarioId == usuarioId &&
                s.CanteiroId == canteiroId &&
                s.Status == StatusSolicitacao.Pendente);

            if (existePendente)
            {
                TempData["MensagemInfo"] = "Você já possui uma Solicitação pendente para esse canteiro.";
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

            var ferramenta = await _db.Ferramentas
                .Include(f => f.Horta)
                .FirstOrDefaultAsync(f => f.Id == ferramentaId);

            if (ferramenta == null) return NotFound();
            if (!await _currentUser.CanManageHortaAsync(ferramenta.Horta)) return Forbid();

            ferramenta.Nome = (nome ?? string.Empty).Trim();

            if (Enum.TryParse<StatusFerramenta>(status, ignoreCase: true, out var st))
            {
                ferramenta.Status = st;
            }

            await _db.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Ferramenta atualizada.";
            return RedirectToAction("Details", "Horta", new { id = ferramenta.HortaId });
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

            var ferramenta = await _db.Ferramentas
                .Include(f => f.Horta)
                .FirstOrDefaultAsync(f => f.Id == ferramentaId);

            if (ferramenta == null) return NotFound();
            if (!await _currentUser.CanManageHortaAsync(ferramenta.Horta)) return Forbid();

            if (ferramenta.Status == StatusFerramenta.Inativa)
                ferramenta.Status = StatusFerramenta.Disponivel;
            else
                ferramenta.Status = StatusFerramenta.Inativa;

            await _db.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Status da ferramenta atualizado.";
            return RedirectToAction("Details", "Horta", new { id = ferramenta.HortaId });
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

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();
            var horta = s.Canteiro?.Horta;
            if (horta == null) return NotFound();
            if (!await _currentUser.CanManageHortaAsync(horta)) return Forbid();

            if (s.Status != StatusSolicitacao.Pendente)
            {
                TempData["MensagemInfo"] = "Solicitação já processada.";
                return RedirectToAction("Manage", "Horta", new { id = horta.Id });
            }

            s.Status = StatusSolicitacao.Aprovado;
            s.Canteiro.Status = StatusCanteiro.Ocupado;

            var outros = await _db.Solicitacoes
                .Where(x => x.CanteiroId == s.CanteiroId && x.Status == StatusSolicitacao.Pendente && x.Id != s.Id)
                .ToListAsync();
            foreach (var o in outros) o.Status = StatusSolicitacao.Recusado;

            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Solicitação aprovada. Participante confirmado.";
            return RedirectToAction("Manage", "Horta", new { id = horta.Id });
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

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();
            var horta = s.Canteiro?.Horta;
            if (horta == null) return NotFound();
            if (!await _currentUser.CanManageHortaAsync(horta)) return Forbid();

            if (s.Status != StatusSolicitacao.Pendente)
            {
                TempData["MensagemInfo"] = "Solicitação já processada.";
                return RedirectToAction("Manage", "Horta", new { id = horta.Id });
            }

            s.Status = StatusSolicitacao.Recusado;
            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Solicitação recusada.";
            return RedirectToAction("Manage", "Horta", new { id = horta.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sair(Guid solicitacaoId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para gerenciar participantes.";
                return RedirectToAction("Login", "Account");
            }

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();
            var horta = s.Canteiro?.Horta;
            if (horta == null) return NotFound();
            if (!await _currentUser.CanManageHortaAsync(horta)) return Forbid();

            if (s.Status != StatusSolicitacao.Aprovado)
            {
                TempData["MensagemInfo"] = "Este participante não está mais ativo neste canteiro.";
                return RedirectToAction("Manage", "Horta", new { id = horta.Id });
            }

            s.Status = StatusSolicitacao.Cancelado;
            s.Canteiro.Status = StatusCanteiro.Disponivel;

            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Participante removido do canteiro.";
            return RedirectToAction("Manage", "Horta", new { id = horta.Id });
        }
    }

}