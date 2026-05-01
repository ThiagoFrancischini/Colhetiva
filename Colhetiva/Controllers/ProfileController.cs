using Microsoft.AspNetCore.Mvc;
using Colhetiva.DTOs;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.Core.Entities;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Colhetiva.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ColhetivaDbContext _db;

        public ProfileController(IUsuarioService usuarioService, ColhetivaDbContext db)
        {
            _usuarioService = usuarioService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                usuarioIdStr = TempData["UsuarioId"]?.ToString();
            }
            
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var usuarioId = Guid.Parse(usuarioIdStr);
            var usuario = await _db.Usuarios
                .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
                .ThenInclude(c => c.Estado)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userContexts = await _db.UserContexts.Where(uc => uc.UsuarioId == usuarioId).ToListAsync();
            var role = userContexts.FirstOrDefault()?.Role.ToString() ?? "PARTICIPANT";

            // Load cidades for dropdown
            var cidades = await _db.Cidades.ToListAsync();
            ViewBag.Cidades = new SelectList(cidades, "Id", "Nome", usuario.Endereco?.CidadeId);

            var model = new ProfileViewDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                CPF = usuario.CPF ?? "",
                Role = role,
                Endereco = new EnderecoDto
                {
                    Id = usuario.Endereco?.Id ?? Guid.Empty,
                    Cep = usuario.Endereco?.Cep ?? "",
                    Rua = usuario.Endereco?.Rua ?? "",
                    Numero = usuario.Endereco?.Numero ?? "",
                    Bairro = usuario.Endereco?.Bairro ?? "",
                    CidadeId = usuario.Endereco?.CidadeId ?? Guid.Empty,
                    CidadeNome = usuario.Endereco?.Cidade?.Nome ?? "",
                    EstadoNome = usuario.Endereco?.Cidade?.Estado?.Nome ?? ""
                }
            };

            ViewBag.IsAdmin = role == "ADMIN";
            ViewBag.IsMaster = role == "MASTER";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileUpdateDto model)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                usuarioIdStr = TempData["UsuarioId"]?.ToString();
            }
            
            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var usuarioId = Guid.Parse(usuarioIdStr);
            var usuario = await _db.Usuarios
                .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Remove CPF validation if not needed (e.g., for organizations)
            var userContexts = await _db.UserContexts.Where(uc => uc.UsuarioId == usuarioId).ToListAsync();
            var role = userContexts.FirstOrDefault()?.Role.ToString() ?? "PARTICIPANT";
            
            if (role == "ADMIN" || string.IsNullOrEmpty(model.CPF))
            {
                ModelState.Remove("CPF");
            }

            // Load cidades for dropdown
            var cidades = await _db.Cidades.ToListAsync();
            ViewBag.Cidades = new SelectList(cidades, "Id", "Nome", model.Endereco?.CidadeId);

            if (!ModelState.IsValid)
            {
                // Reconstruir ProfileViewDto para a view
                var viewModel = new ProfileViewDto
                {
                    Id = usuario.Id,
                    Nome = model.Nome,
                    Email = usuario.Email,
                    CPF = model.CPF,
                    Role = role,
                    Endereco = new EnderecoDto
                    {
                        Id = usuario.Endereco?.Id ?? Guid.Empty,
                        Cep = model.Endereco?.Cep ?? "",
                        Rua = model.Endereco?.Rua ?? "",
                        Numero = model.Endereco?.Numero ?? "",
                        Bairro = model.Endereco?.Bairro ?? "",
                        CidadeId = model.Endereco?.CidadeId ?? Guid.Empty,
                        CidadeNome = usuario.Endereco?.Cidade?.Nome ?? "",
                        EstadoNome = usuario.Endereco?.Cidade?.Estado?.Nome ?? ""
                    }
                };
                
                ViewBag.IsAdmin = role == "ADMIN";
                ViewBag.IsMaster = role == "MASTER";
                
                return View(viewModel);
            }

            try
            {
                // Atualizar dados do usuário
                usuario.Nome = model.Nome;
                
                // Atualizar endereço
                if (usuario.Endereco != null && model.Endereco != null)
                {
                    // Limpar máscara do CEP (remover hífen)
                    var cepLimpo = new string(model.Endereco.Cep?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                    if (cepLimpo.Length != 8)
                    {
                        ModelState.AddModelError("Endereco.Cep", "CEP deve ter 8 dígitos.");
                        // Reconstruir viewModel e retornar
                        var viewModel = new ProfileViewDto
                        {
                            Id = usuario.Id,
                            Nome = model.Nome,
                            Email = usuario.Email,
                            CPF = model.CPF,
                            Role = role,
                            Endereco = new EnderecoDto
                            {
                                Id = usuario.Endereco?.Id ?? Guid.Empty,
                                Cep = model.Endereco?.Cep ?? "",
                                Rua = model.Endereco?.Rua ?? "",
                                Numero = model.Endereco?.Numero ?? "",
                                Bairro = model.Endereco?.Bairro ?? "",
                                CidadeId = model.Endereco?.CidadeId ?? Guid.Empty,
                                CidadeNome = usuario.Endereco?.Cidade?.Nome ?? "",
                                EstadoNome = usuario.Endereco?.Cidade?.Estado?.Nome ?? ""
                            }
                        };
                        ViewBag.IsAdmin = role == "ADMIN";
                        ViewBag.IsMaster = role == "MASTER";
                        ViewBag.Cidades = new SelectList(cidades, "Id", "Nome", model.Endereco?.CidadeId);
                        return View(viewModel);
                    }
                    
                    usuario.Endereco.Cep = cepLimpo;
                    usuario.Endereco.Rua = model.Endereco.Rua;
                    usuario.Endereco.Numero = model.Endereco.Numero;
                    usuario.Endereco.Bairro = model.Endereco.Bairro;
                    usuario.Endereco.CidadeId = model.Endereco.CidadeId;
                }
                
                // Salvar usando o mesmo contexto que carregou a entidade
                await _db.SaveChangesAsync();

                TempData["MensagemSucesso"] = "Perfil atualizado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log full exception chain for debugging
                ModelState.AddModelError(string.Empty, $"Erro ao atualizar perfil: {ex}");
                
                // Reconstruir ProfileViewDto para a view em caso de erro
                var viewModel = new ProfileViewDto
                {
                    Id = usuario.Id,
                    Nome = model.Nome,
                    Email = usuario.Email,
                    CPF = model.CPF,
                    Role = role,
                    Endereco = new EnderecoDto
                    {
                        Id = usuario.Endereco?.Id ?? Guid.Empty,
                        Cep = model.Endereco?.Cep ?? "",
                        Rua = model.Endereco?.Rua ?? "",
                        Numero = model.Endereco?.Numero ?? "",
                        Bairro = model.Endereco?.Bairro ?? "",
                        CidadeId = model.Endereco?.CidadeId ?? Guid.Empty,
                        CidadeNome = usuario.Endereco?.Cidade?.Nome ?? "",
                        EstadoNome = usuario.Endereco?.Cidade?.Estado?.Nome ?? ""
                    }
                };
                
                ViewBag.IsAdmin = role == "ADMIN";
                ViewBag.IsMaster = role == "MASTER";
                
                return View(viewModel);
            }
        }
    }
}