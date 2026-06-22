using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHortaService _hortaService;
        private readonly ColhetivaDbContext _db;

        public HomeController(IHortaService hortaService, ColhetivaDbContext db)
        {
            _hortaService = hortaService;
            _db = db;
        }

        public async Task<IActionResult> Index(string nome, string cidade)
        {
            var hortas = await _hortaService.FiltrarAsync(nome, cidade);

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (!string.IsNullOrEmpty(usuarioIdStr))
            {
                var usuarioId = Guid.Parse(usuarioIdStr);
                var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);
                if (usuario != null && usuario.OrganizationId.HasValue)
                {
                    var orgId = usuario.OrganizationId.Value;
                    hortas = hortas.Where(h => h.OrganizationId.HasValue && h.OrganizationId.Value == orgId).ToList();
                }
            }

            var vm = new HomeDto
            {
                NomeUsuario = HttpContext.Session.GetString("UsuarioNome"),
                Hortas = hortas.Select(HortaCardDto.FromEntity).ToList()
            };

            return View(vm);
        }
    }
}