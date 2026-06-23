using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Colhetiva.DTOs;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.Core.Enums;
using Colhetiva.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Colhetiva.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Colhetiva.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEnderecoService _enderecoService;
        private readonly IHortaService _hortaService;
        private readonly ColhetivaDbContext _db;

        public AccountController(
            IUsuarioService usuarioService, 
            IEnderecoService enderecoService,
            IHortaService hortaService,
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

                var userContexts = await _db.UserContexts.Where(uc => uc.UsuarioId == usuario.Id).ToListAsync();
                var role = userContexts.FirstOrDefault()?.Role ?? Role.PARTICIPANT;

                HttpContext.Session.SetString("UsuarioNome", usuario.Nome ?? usuario.Email);
                HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                HttpContext.Session.SetString("UsuarioRole", role.ToString());

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

            if (!ModelState.IsValid)
                return View(model);

            if (model.Endereco == null || model.Endereco.CidadeId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Cidade não foi selecionada corretamente. Por favor, selecione uma cidade.");
                return View(model);
            }

            model.Endereco.Cep = new string(model.Endereco.Cep?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
            if (model.Endereco.Cep.Length != 8)
            {
                ModelState.AddModelError("Endereco.Cep", "CEP deve ter 8 dígitos.");
                return View(model);
            }

            try
            {
                // salvar endereço da organização
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

                // criar organização
                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Nome = model.Nome,
                    Cnpj = string.Empty, // se DTO tiver CNPJ, atribua aqui (model.Cnpj)
                    Tipo = model.TipoOrganizacao ?? string.Empty,
                    EnderecoId = endereco.Id,
                    Endereco = endereco
                };

                await _db.Organizations.AddAsync(organization);
                await _db.SaveChangesAsync();

                // criar usuário responsável vinculado à organização
                var usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = model.Nome,
                    CPF = string.Empty, // CPF não exigido para organização responsável
                    Email = model.Email,
                    Password = model.Password,
                    EnderecoId = endereco.Id,
                    Endereco = endereco,
                    OrganizationId = organization.Id
                };

                // Persistir usuário direto no DbContext para não acionar a validação de CPF em UsuarioService
                await _db.Usuarios.AddAsync(usuario);
                await _db.SaveChangesAsync();

                // criar UserContext como ADMIN
                var uc = new UserContext
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Role = Role.ADMIN,
                    HortaId = null
                };
                await _db.UserContexts.AddAsync(uc);
                await _db.SaveChangesAsync();

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
        // Substitua apenas o método Index existente por este
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtém todas as hortas via serviço
            var todasHortas = await _hortaService.GetAllAsync();

            // Se houver usuário logado, tenta filtrar por OrganizationId vinculada ao usuário
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (!string.IsNullOrEmpty(usuarioIdStr))
            {
                if (Guid.TryParse(usuarioIdStr, out var usuarioId))
                {
                    var usuario = await _db.Usuarios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == usuarioId);

                    if (usuario != null && usuario.OrganizationId.HasValue)
                    {
                        var orgId = usuario.OrganizationId.Value;
                        var hortasDaOrg = todasHortas
                            .Where(h => h.OrganizationId.HasValue && h.OrganizationId.Value == orgId)
                            .ToList();

                        return View(hortasDaOrg);
                    }
                }
            }

            // Se não houver usuário com organização, exibe todas (ou mantenha política desejada)
            return View(todasHortas);
        }
    }
}