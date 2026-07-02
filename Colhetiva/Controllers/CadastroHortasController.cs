using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Core.Input;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.Filters;
using Colhetiva.Infrastructure.Context;
using Colhetiva.Services;
using Colhetiva.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Controllers;

[RequireAdmin]
public class CadastroHortasController : Controller
{
    private readonly IHortaService _hortaService;
    private readonly ColhetivaDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CadastroHortasController(
        IHortaService hortaService,
        ColhetivaDbContext db,
        ICurrentUserService currentUser)
    {
        _hortaService = hortaService;
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public IActionResult NovoCanteiroLinha(int i)
    {
        return PartialView("_LinhaCanteiro", new CanteiroLinhaEditorModel { Index = i, Linha = new CanteiroLinhaViewModel() });
    }

    [HttpGet]
    public IActionResult NovoFerramentaLinha(int i)
    {
        return PartialView("_LinhaFerramenta", new FerramentaLinhaEditorModel { Index = i, Linha = new FerramentaLinhaViewModel() });
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var todasHortas = await _hortaService.GetAllAsync();

        var organizationId = await _currentUser.GetOrganizationIdAsync();
        if (organizationId.HasValue)
        {
            var hortasDaOrg = todasHortas
                .Where(h => h.OrganizationId == organizationId.Value)
                .ToList();

            return View(hortasDaOrg);
        }

        // Se não houver usuário com organização, exibe todas (ex.: ADMIN de sistema)
        return View(todasHortas);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopularSelectsAsync();
        var model = new HortaCadastroViewModel
        {
            Canteiros = new List<CanteiroLinhaViewModel> { new() },
            Ferramentas = new List<FerramentaLinhaViewModel> { new() }
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HortaCadastroViewModel model)
    {
        await PopularSelectsAsync();
        NormalizarCep(model);
        ValidarCepModelState(model);
        model.Canteiros = model.Canteiros?.Where(c => c != null).ToList() ?? new();
        model.Ferramentas = model.Ferramentas?.Where(f => f != null).ToList() ?? new();
        if (!ModelState.IsValid)
            return View(model);

        var responsavelId = model.UsuarioId ?? _currentUser.UsuarioId;
        if (responsavelId == null)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível determinar o responsável pela horta.");
            return View(model);
        }

        try
        {
            var input = ToInput(model, responsavelId.Value);
            var organizationId = await _currentUser.GetOrganizationIdAsync();
            await _hortaService.CriarCompletoAsync(input, organizationId);
            TempData["MensagemSucesso"] = "Horta cadastrada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var horta = await _hortaService.GetByIdAsync(id);
        if (horta == null)
            return NotFound();

        await PopularSelectsAsync();
        var model = ToViewModel(horta);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, HortaCadastroViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        await PopularSelectsAsync();
        NormalizarCep(model);
        ValidarCepModelState(model);
        model.Canteiros = model.Canteiros?.Where(c => c != null).ToList() ?? new();
        model.Ferramentas = model.Ferramentas?.Where(f => f != null).ToList() ?? new();
        if (!ModelState.IsValid)
            return View(model);

        var responsavelId = model.UsuarioId ?? _currentUser.UsuarioId;
        if (responsavelId == null)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível determinar o responsável pela horta.");
            return View(model);
        }

        try
        {
            var input = ToInput(model, responsavelId.Value);
            var organizationId = await _currentUser.GetOrganizationIdAsync();
            await _hortaService.AtualizarCompletoAsync(input, organizationId);
            TempData["MensagemSucesso"] = "Horta atualizada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var horta = await _hortaService.GetByIdAsync(id);
        if (horta == null)
            return NotFound();
        return View(horta);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _hortaService.ExcluirAsync(id);
        TempData["MensagemSucesso"] = "Horta excluída.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopularSelectsAsync()
    {
        var cidades = await _db.Cidades.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        ViewBag.Cidades = new SelectList(cidades, "Id", "Nome");

        var participantes = await _db.Usuarios
            .AsNoTracking()
            .Where(u => u.UserContexts.Any(uc => uc.Role == Role.PARTICIPANT) && !u.OrganizationId.HasValue)
            .OrderBy(u => u.Nome ?? u.Email)
            .Select(u => new { u.Id, Text = string.IsNullOrWhiteSpace(u.Nome) ? u.Email : $"{u.Nome} ({u.Email})" })
            .ToListAsync();
        ViewBag.Usuarios = new SelectList(participantes, "Id", "Text");
    }

    private static void NormalizarCep(HortaCadastroViewModel model)
    {
        if (model.Endereco?.Cep == null)
            return;
        model.Endereco.Cep = new string(model.Endereco.Cep.Where(char.IsDigit).ToArray());
    }

    private void ValidarCepModelState(HortaCadastroViewModel model)
    {
        if (model.Endereco?.Cep == null || model.Endereco.Cep.Length != 8)
            ModelState.AddModelError("Endereco.Cep", "CEP deve ter 8 dígitos.");
    }

    private static HortaCadastroInput ToInput(HortaCadastroViewModel vm, Guid usuarioResponsavelId)
    {
        return new HortaCadastroInput
        {
            Id = vm.Id,
            Nome = vm.Nome.Trim(),
            Regras = vm.Regras?.Trim() ?? string.Empty,
            UsuarioId = usuarioResponsavelId,
            Endereco = new EnderecoHortaInput
            {
                Id = vm.Endereco.Id,
                Cep = vm.Endereco.Cep ?? string.Empty,
                Rua = vm.Endereco.Rua ?? string.Empty,
                Numero = vm.Endereco.Numero ?? string.Empty,
                Bairro = vm.Endereco.Bairro ?? string.Empty,
                Complemento = vm.Endereco.Complemento ?? string.Empty,
                Latitude = vm.Endereco.Latitude,
                Longitude = vm.Endereco.Longitude,
                CidadeId = vm.Endereco.CidadeId
            },
            Canteiros = (vm.Canteiros ?? new List<CanteiroLinhaViewModel>())
                .Select(c => new CanteiroLinhaInput
                {
                    Id = c.Id,
                    Identificacao = c.Identificacao ?? string.Empty,
                    Dimensoes = c.Dimensoes ?? string.Empty,
                    Status = c.Status,
                    Remover = c.Remover
                }).ToList(),
            Ferramentas = (vm.Ferramentas ?? new List<FerramentaLinhaViewModel>())
                .Select(f => new FerramentaLinhaInput
                {
                    Id = f.Id,
                    Nome = f.Nome ?? string.Empty,
                    Status = f.Status,
                    Remover = f.Remover
                }).ToList()
        };
    }

    private static HortaCadastroViewModel ToViewModel(Horta h)
    {
        var end = h.Endereco;
        return new HortaCadastroViewModel
        {
            Id = h.Id,
            Nome = h.Nome,
            Regras = h.Regras ?? string.Empty,
            UsuarioId = h.UsuarioId,
            Endereco = new EnderecoHortaViewModel
            {
                Id = end?.Id ?? Guid.Empty,
                Cep = end?.Cep ?? string.Empty,
                Rua = end?.Rua ?? string.Empty,
                Numero = end?.Numero ?? string.Empty,
                Bairro = end?.Bairro ?? string.Empty,
                Complemento = end?.Complemento ?? string.Empty,
                Latitude = end?.Latitude ?? 0,
                Longitude = end?.Longitude ?? 0,
                CidadeId = end?.CidadeId ?? Guid.Empty
            },
            Canteiros = h.Canteiros
                .OrderBy(c => c.Identificacao)
                .Select(c => new CanteiroLinhaViewModel
                {
                    Id = c.Id,
                    Identificacao = c.Identificacao,
                    Dimensoes = c.Dimensoes ?? string.Empty,
                    Status = c.Status,
                    Remover = false
                }).ToList(),
            Ferramentas = h.Ferramentas
                .OrderBy(f => f.Nome)
                .Select(f => new FerramentaLinhaViewModel
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Status = f.Status,
                    Remover = false
                }).ToList()
        };
    }
}
