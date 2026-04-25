using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmprestimosController : ControllerBase
{
    private readonly IEmprestimoService _emprestimoService;

    public EmprestimosController(IEmprestimoService emprestimoService)
    {
        _emprestimoService = emprestimoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmprestimoDto>>> GetAll()
    {
        var emprestimos = await _emprestimoService.GetAllAsync();
        return Ok(emprestimos.Select(e => e.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmprestimoDto>> GetById(Guid id)
    {
        var emprestimo = await _emprestimoService.GetByIdAsync(id);
        if (emprestimo == null) return NotFound();
        return Ok(emprestimo.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<EmprestimoDto>> Create([FromBody] EmprestimoCreateDto dto)
    {
        var emprestimo = dto.ToEntity();
        var created = await _emprestimoService.CriarEmprestimoAsync(emprestimo);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }
}