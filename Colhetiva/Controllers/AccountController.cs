using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Colhetiva.DTOs;
using Colhetiva.Core.Interfaces.Service;

namespace Colhetiva.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AccountController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
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

                // Exemplo simples: armazenar identificação em TempData (substitua por autenticação real)
                TempData["UsuarioNome"] = usuario.Nome ?? usuario.Email;
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Erro ao autenticar. Tente novamente.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            TempData.Remove("UsuarioNome");
            return RedirectToAction("Login");
        }
    }
}