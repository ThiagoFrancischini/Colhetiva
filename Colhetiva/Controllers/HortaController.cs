using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Core.Enums;

namespace Colhetiva.Controllers
{
    public class HortaController : Controller
    {
        private readonly ColhetivaDbContext _db;

        public HortaController(ColhetivaDbContext db)
        {
            _db = db;
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

            // Identifica canteiros com solicitação pendente do usuário logado
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
            return View(horta);
        }
    }
}