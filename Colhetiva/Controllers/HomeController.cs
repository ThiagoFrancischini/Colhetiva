using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Colhetiva.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHortaService _hortaService;

        public HomeController(IHortaService hortaService)
        {
            _hortaService = hortaService;
        }

        public async Task<IActionResult> Index(string nome, string cidade)
        {
            var hortas = await _hortaService.FiltrarAsync(nome, cidade);

            var vm = new HomeDto
            {
                NomeUsuario = HttpContext.Session.GetString("UsuarioNome"),
                Hortas = hortas.Select(HortaCardDto.FromEntity).ToList()
            };

            return View(vm);
        }
    }
}