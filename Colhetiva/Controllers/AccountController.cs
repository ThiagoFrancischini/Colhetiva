using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Colhetiva.DTOs;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Colhetiva.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ColhetivaDbContext _db;

        public AccountController(IUsuarioService usuarioService, ColhetivaDbContext db)
        {
            _usuarioService = usuarioService;
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new UsuarioLoginDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UsuarioLoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var usuario = await _usuarioService.Autenticar(model.Email, model.Password);

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                    return View(model);
                }

                TempData["UsuarioNome"] = usuario.Nome ?? usuario.Email;
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Erro ao autenticar. Tente novamente.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var cidades = await _db.Cidades.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
            ViewBag.Cidades = new SelectList(cidades, "Id", "Nome");
            var model = new UsuarioCreateDto { Endereco = new EnderecoCreateDto() };
            return View(model);
        }

        public IActionResult Logout()
        {
            TempData.Remove("UsuarioNome");
            return RedirectToAction("Login");
        }
    }
}