using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Colhetiva.DTOs;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.Core.Enums;
using Colhetiva.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Colhetiva.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEnderecoService _enderecoService;
        private readonly ColhetivaDbContext _db;

        public AccountController(
            IUsuarioService usuarioService, 
            IEnderecoService enderecoService,
            ColhetivaDbContext db)
        {
            _usuarioService = usuarioService;
            _enderecoService = enderecoService;
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

                // Buscar a role do usuário
                var userContexts = await _db.UserContexts.Where(uc => uc.UsuarioId == usuario.Id).ToListAsync();
                var role = userContexts.FirstOrDefault()?.Role ?? Role.PARTICIPANT;

                // Armazenar na Session
                HttpContext.Session.SetString("UsuarioNome", usuario.Nome ?? usuario.Email);
                HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                HttpContext.Session.SetString("UsuarioRole", role.ToString());

                // Também manter no TempData para compatibilidade
                TempData["UsuarioNome"] = usuario.Nome ?? usuario.Email;
                TempData["UsuarioId"] = usuario.Id.ToString();
                TempData["UsuarioRole"] = role.ToString();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UsuarioCreateDto model)
        {
            var cidades = await _db.Cidades.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
            ViewBag.Cidades = new SelectList(cidades, "Id", "Nome");

            if (!ModelState.IsValid)
                return View(model);

            // Limpar máscaras (backup se o JS não funcionar)
            if (model.Endereco != null)
            {
                model.Endereco.Cep = new string(model.Endereco.Cep?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                if (model.Endereco.Cep.Length != 8)
                {
                    ModelState.AddModelError("Endereco.Cep", "CEP deve ter 8 dígitos.");
                    return View(model);
                }
                
                if (model.Endereco.CidadeId == Guid.Empty)
                {
                    ModelState.AddModelError("Endereco.CidadeId", "Selecione uma cidade.");
                    return View(model);
                }
            }
            
            // Limpar máscara do CPF
            if (!string.IsNullOrEmpty(model.CPF))
            {
                model.CPF = new string(model.CPF.Where(char.IsDigit).ToArray());
                if (model.CPF.Length != 11)
                {
                    ModelState.AddModelError("CPF", "CPF deve ter 11 dígitos.");
                    return View(model);
                }
            }

            try
            {
                var endereco = new Endereco
                {
                    Id = Guid.NewGuid(),
                    Cep = model.Endereco?.Cep ?? "",
                    Rua = model.Endereco?.Rua ?? "",
                    Numero = model.Endereco?.Numero ?? "",
                    Bairro = model.Endereco?.Bairro ?? "",
                    Complemento = model.Endereco?.Complemento ?? "",
                    CidadeId = model.Endereco?.CidadeId ?? Guid.Empty,
                    Latitude = 0,
                    Longitude = 0
                };

                await _enderecoService.Salvar(endereco);

                var usuario = new Usuario
                {
                    Nome = model.Nome,
                    CPF = model.CPF ?? "",
                    Email = model.Email,
                    Password = model.Password,
                    EnderecoId = endereco.Id,
                    Endereco = endereco
                };

                await _usuarioService.Salvar(usuario);

                TempData["MensagemSucesso"] = "Conta criada com sucesso! Faça login para continuar.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, TratarExcecao(ex));
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> RegisterOrganization()
        {
            var cidades = await _db.Cidades.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
            ViewBag.Cidades = new SelectList(cidades, "Id", "Nome");
            var model = new OrganizationCreateDto { Endereco = new EnderecoCreateDto() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOrganization(OrganizationCreateDto model)
        {
            var cidades = await _db.Cidades.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
            ViewBag.Cidades = new SelectList(cidades, "Id", "Nome");

            // DEBUG: Log dos valores recebidos
            System.Diagnostics.Debug.WriteLine($"=== DEBUG REGISTER ORGANIZATION ===");
            System.Diagnostics.Debug.WriteLine($"Nome: {model.Nome}");
            System.Diagnostics.Debug.WriteLine($"Email: {model.Email}");
            System.Diagnostics.Debug.WriteLine($"CidadeId: {model.Endereco?.CidadeId}");
            System.Diagnostics.Debug.WriteLine($"CidadeId is empty: {model.Endereco?.CidadeId == Guid.Empty}");
            System.Diagnostics.Debug.WriteLine($"Model Endereco is null: {model.Endereco == null}");
            System.Diagnostics.Debug.WriteLine($"==================================");

            if (!ModelState.IsValid)
                return View(model);

            // Se veio vazio, não deixa passar
            if (model.Endereco == null || model.Endereco.CidadeId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Cidade não foi selecionada corretamente. Por favor, selecione uma cidade.");
                return View(model);
            }

            // Limpar máscaras (backup se o JS não funcionar)
            model.Endereco.Cep = new string(model.Endereco.Cep?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
            if (model.Endereco.Cep.Length != 8)
            {
                ModelState.AddModelError("Endereco.Cep", "CEP deve ter 8 dígitos.");
                return View(model);
            }

            try
            {
                var endereco = new Endereco
                {
                    Id = Guid.NewGuid(),
                    Cep = model.Endereco?.Cep ?? "",
                    Rua = model.Endereco?.Rua ?? "",
                    Numero = model.Endereco?.Numero ?? "",
                    Bairro = model.Endereco?.Bairro ?? "",
                    Complemento = model.Endereco?.Complemento ?? "",
                    CidadeId = model.Endereco?.CidadeId ?? Guid.Empty,
                    Latitude = 0,
                    Longitude = 0
                };

                await _enderecoService.Salvar(endereco);

                var usuario = new Usuario
                {
                    Nome = model.Nome,
                    CPF = "",
                    Email = model.Email,
                    Password = model.Password,
                    EnderecoId = endereco.Id,
                    Endereco = endereco
                };

                await _usuarioService.Salvar(usuario, Role.ADMIN);

                TempData["MensagemSucesso"] = "Organização cadastrada com sucesso! Faça login para acessar o painel administrativo.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, TratarExcecao(ex));
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            return RedirectToAction("Login");
        }

        private string TratarExcecao(Exception ex)
        {
            if (ex is InvalidOperationException invalidOp)
            {
                if (invalidOp.Message.Contains("Email já está em uso"))
                    return "Este e-mail já está cadastrado. Tente fazer login ou utilize outro e-mail.";
                return invalidOp.Message;
            }

            if (ex is ArgumentException argEx)
                return argEx.Message;

            var innerMessage = ex.InnerException?.Message ?? ex.Message;

            if (innerMessage.Contains("duplicate key") || innerMessage.Contains("unique"))
                return "Já existe um registro com esses dados. Verifique se o e-mail já está cadastrado.";

            if (innerMessage.Contains("foreign key") || innerMessage.Contains("CidadeId"))
                return "A cidade selecionada não existe. Por favor, selecione uma cidade válida.";

            if (innerMessage.Contains("entity changes") || innerMessage.Contains("DbUpdate"))
                return "Erro ao salvar no banco de dados. Verifique os campos obrigatórios.";

            return $"Erro: {innerMessage}";
        }
    }
}