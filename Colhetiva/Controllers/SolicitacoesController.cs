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

            // IDs de canteiros para os quais o usuário já tem solicitação pendente
            var pendentes = await _db.Solicitacoes
                .Where(s => s.UsuarioId == usuarioId && s.Status == StatusSolicitacao.Pendente)
                .Select(s => s.CanteiroId)
                .ToListAsync();

            ViewBag.PendingCanteiros = pendentes;
            return View(horta);
        }

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

        // --- NOVAS AÇÕES: Gerenciar / Aprovar / Reprovar ---

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

            // Solicitações pendentes relativas às hortas cujo responsável é o usuário atual
            var solicitacoes = await _db.Solicitacoes
                .Include(s => s.Canteiro)
                    .ThenInclude(c => c.Horta)
                .Include(s => s.Usuario)
                .Where(s => s.Canteiro.Horta.UsuarioId == usuarioId && s.Status == StatusSolicitacao.Pendente)
                .OrderBy(s => s.DataPedido)
                .ToListAsync();

            return View(solicitacoes);
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

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();
            if (s.Canteiro.Horta.UsuarioId != usuarioId) return Forbid();

            if (s.Status != StatusSolicitacao.Pendente)
            {
                TempData["MensagemInfo"] = "Solicitação já processada.";
                return RedirectToAction("Manage");
            }

            // Aprovar
            s.Status = StatusSolicitacao.Aprovado;

            // Atualizar canteiro para ocupado
            s.Canteiro.Status = StatusCanteiro.Ocupado;

            // Reprovar outras solicitações pendentes para o mesmo canteiro
            var outras = await _db.Solicitacoes
                .Where(x => x.CanteiroId == s.CanteiroId && x.Status == StatusSolicitacao.Pendente && x.Id != s.Id)
                .ToListAsync();

            foreach (var o in outras)
                o.Status = StatusSolicitacao.Recusado;

            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Solicitação aprovada com sucesso.";
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

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();
            if (s.Canteiro.Horta.UsuarioId != usuarioId) return Forbid();

            if (s.Status != StatusSolicitacao.Pendente)
            {
                TempData["MensagemInfo"] = "Solicitação já processada.";
                return RedirectToAction("Manage");
            }

            s.Status = StatusSolicitacao.Recusado;
            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Solicitação reprovada.";
            return RedirectToAction("Manage");
        }
    }
    
}