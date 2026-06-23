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

        // ... métodos Index / Solicitar mantidos ...

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

            // carregar usuário para obter OrganizationId (se houver)
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            // solicitações de canteiro pendentes
            var solicitacoes = await _db.Solicitacoes
                .Include(s => s.Canteiro)
                    .ThenInclude(c => c.Horta)
                .Include(s => s.Usuario)
                .Where(s => s.Status == StatusSolicitacao.Pendente &&
                            (s.Canteiro.Horta.UsuarioId == usuarioId ||
                             (usuario != null && usuario.OrganizationId.HasValue &&
                              s.Canteiro.Horta.OrganizationId.HasValue &&
                              s.Canteiro.Horta.OrganizationId == usuario.OrganizationId)))
                .OrderBy(s => s.DataPedido)
                .ToListAsync();

            // pedidos de empréstimo pendentes
            var emprestimos = await _db.Emprestimos
                .Include(e => e.Ferramenta)
                    .ThenInclude(f => f.Horta)
                .Include(e => e.Usuario)
                .Where(e => e.Status == StatusEmprestimo.Pendente &&
                            (e.Ferramenta.Horta.UsuarioId == usuarioId ||
                             (usuario != null && usuario.OrganizationId.HasValue &&
                              e.Ferramenta.Horta.OrganizationId.HasValue &&
                              e.Ferramenta.Horta.OrganizationId == usuario.OrganizationId)))
                .OrderBy(e => e.DataRetirada)
                .ToListAsync();

            // participantes atuais (solicitações aprovadas)
            var participantes = await _db.Solicitacoes
                .Include(s => s.Canteiro)
                    .ThenInclude(c => c.Horta)
                .Include(s => s.Usuario)
                .Where(s => s.Status == StatusSolicitacao.Aprovado &&
                            (s.Canteiro.Horta.UsuarioId == usuarioId ||
                             (usuario != null && usuario.OrganizationId.HasValue &&
                              s.Canteiro.Horta.OrganizationId.HasValue &&
                              s.Canteiro.Horta.OrganizationId == usuario.OrganizationId)))
                .OrderBy(s => s.DataPedido)
                .ToListAsync();

            ViewBag.PendingEmprestimos = emprestimos;
            ViewBag.Participantes = participantes;

            return View(solicitacoes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sair(Guid solicitacaoId, Guid? hortaId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["MensagemInfo"] = "Faça login para cancelar a participação.";
                return RedirectToAction("Login", "Account");
            }
            var usuarioId = Guid.Parse(usuarioIdStr);

            var s = await _db.Solicitacoes
                .Include(x => x.Canteiro)
                    .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(x => x.Id == solicitacaoId);

            if (s == null) return NotFound();

            var horta = s.Canteiro?.Horta;
            if (horta == null) return NotFound();

            // quem pode remover: o próprio solicitante OU o responsável da horta OU membro da organização
            var usuario = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);
            var pertenceOrg = usuario != null && usuario.OrganizationId.HasValue && horta.OrganizationId.HasValue && usuario.OrganizationId == horta.OrganizationId;
            var isGestor = horta.UsuarioId == usuarioId || pertenceOrg;
            var isSolicitante = s.UsuarioId == usuarioId;

            if (!isSolicitante && !isGestor) return Forbid();

            // se estava aprovado (ocupando), liberar o canteiro
            if (s.Status == StatusSolicitacao.Aprovado)
            {
                s.Canteiro.Status = StatusCanteiro.Disponivel;
            }

            s.Status = StatusSolicitacao.Cancelado;
            await _db.SaveChangesAsync();

            TempData["MensagemSucesso"] = isSolicitante ? "Você saiu do canteiro." : "Participante removido.";
            return RedirectToAction("Manage");
        }

        // Aprovar / Reprovar já existentes (mantidos) ...
        // (Aprovar / Reprovar tratam Pendente -> Aprovado/Recusado)
    }
    
}