using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Services;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHortaService _hortaService;
        private readonly IAvisoService _avisoService;
        private readonly ICurrentUserService _currentUser;

        public HomeController(IHortaService hortaService, IAvisoService avisoService, ICurrentUserService currentUser)
        {
            _hortaService = hortaService;
            _avisoService = avisoService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index(string nome, string cidade)
        {
            // Organização usa o painel próprio (CadastroHortas), não a busca pública de hortas.
            if (await _currentUser.IsOrganizationAdminAsync())
            {
                return RedirectToAction("Index", "CadastroHortas");
            }

            // Busca pública de hortas para participantes (filtros de nome/cidade).
            var hortas = await _hortaService.FiltrarAsync(nome, cidade);

            var vm = new HomeDto
            {
                NomeUsuario = HttpContext.Session.GetString("UsuarioNome"),
                Hortas = hortas.Select(HortaCardDto.FromEntity).ToList()
            };

            var usuarioId = _currentUser.UsuarioId;
            if (usuarioId.HasValue)
            {
                var feed = await _avisoService.GetFeedConsolidadoAsync(usuarioId.Value);
                vm.Feed = feed.Select(AvisoFeedDto.FromEntity).ToList();
            }

            return View(vm);
        }
    }
}