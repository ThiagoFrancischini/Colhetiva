using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Core.Enums;
using Colhetiva.Services;

namespace Colhetiva.Controllers
{
    public class HortaController : Controller
    {
        private readonly ColhetivaDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public HortaController(ColhetivaDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
                return RedirectToAction("Index", "Home");

            var horta = await _db.Hortas
                .Include(h => h.Endereco)
                    .ThenInclude(e => e.Cidade)
                        .ThenInclude(c => c.Estado)
                .Include(h => h.Usuario)
                .Include(h => h.Canteiros)
                .Include(h => h.Ferramentas) // <-- carregamento das ferramentas
                .FirstOrDefaultAsync(h => h.Id == id);

            if (horta == null)
                return NotFound();

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            List<Guid> pendentes = new();
            List<Guid> emprestimosAtivos = new();
            if (!string.IsNullOrEmpty(usuarioIdStr))
            {
                var usuarioId = Guid.Parse(usuarioIdStr);
                pendentes = await _db.Solicitacoes
                    .Where(s => s.UsuarioId == usuarioId && s.Status == StatusSolicitacao.Pendente)
                    .Select(s => s.CanteiroId)
                    .ToListAsync();

                emprestimosAtivos = await _db.Emprestimos
                    .Where(e => e.UsuarioId == usuarioId && e.Status == StatusEmprestimo.Aprovado && e.DataDevolucao == null)
                    .Select(e => e.FerramentaId)
                    .ToListAsync();
            }

            ViewBag.PendingCanteiros = pendentes;
            ViewBag.MyActiveEmprestimos = emprestimosAtivos;

            var podeGerenciar = await _currentUser.CanManageHortaAsync(horta);
            ViewBag.PodeGerenciar = podeGerenciar;
            if (podeGerenciar)
            {
                var pendingSolicitacoes = await _db.Solicitacoes
                    .CountAsync(s => s.Status == StatusSolicitacao.Pendente && s.Canteiro.HortaId == id);
                var pendingEmprestimos = await _db.Emprestimos
                    .CountAsync(e => e.Status == StatusEmprestimo.Pendente && e.Ferramenta.HortaId == id);

                ViewBag.PendingManageCount = pendingSolicitacoes + pendingEmprestimos;
            }

            return View(horta);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(Guid id)
        {
            if (id == Guid.Empty)
                return RedirectToAction("Index", "Home");

            var horta = await _db.Hortas
                .Include(h => h.Canteiros)
                .Include(h => h.Ferramentas)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (horta == null)
                return NotFound();

            if (!await _currentUser.CanManageHortaAsync(horta))
            {
                TempData["MensagemInfo"] = "Você não tem permissão para gerenciar esta horta.";
                return RedirectToAction("Details", new { id });
            }

            var solicitacoesPendentes = await _db.Solicitacoes
                .Include(s => s.Usuario)
                .Include(s => s.Canteiro)
                .Where(s => s.Status == StatusSolicitacao.Pendente && s.Canteiro.HortaId == id)
                .OrderBy(s => s.DataPedido)
                .ToListAsync();

            var participantesAtivos = await _db.Solicitacoes
                .Include(s => s.Usuario)
                .Include(s => s.Canteiro)
                .Where(s => s.Status == StatusSolicitacao.Aprovado && s.Canteiro.HortaId == id)
                .OrderBy(s => s.DataPedido)
                .ToListAsync();

            var emprestimosPendentes = await _db.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Ferramenta)
                .Where(e => e.Status == StatusEmprestimo.Pendente && e.Ferramenta.HortaId == id)
                .OrderBy(e => e.DataRetirada)
                .ToListAsync();

            var totalCanteiros = horta.Canteiros.Count;
            var occupiedCanteiros = horta.Canteiros.Count(c => c.Status == StatusCanteiro.Ocupado);

            ViewBag.SolicitacoesPendentes = solicitacoesPendentes;
            ViewBag.ParticipantesAtivos = participantesAtivos;
            ViewBag.EmprestimosPendentes = emprestimosPendentes;
            ViewBag.TotalParticipantsHorta = participantesAtivos.Count;
            ViewBag.TaxaOcupacaoHorta = totalCanteiros == 0 ? 0m : Math.Round((decimal)occupiedCanteiros * 100m / totalCanteiros, 1);
            ViewBag.FerramentasEmUsoHorta = horta.Ferramentas.Count(f => f.Status == StatusFerramenta.EmUso);

            return View(horta);
        }
    }
}