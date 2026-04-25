using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FerramentasController : ControllerBase
{
    private readonly IFerramentaService _ferramentaService;

    public FerramentasController(IFerramentaService ferramentaService)
    {
        _ferramentaService = ferramentaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FerramentaDto>>> GetAll()
    {
        var ferramentas = await _ferramentaService.GetAllAsync();
        return Ok(ferramentas.Select(f => f.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FerramentaDto>> GetById(Guid id)
    {
        var ferramenta = await _ferramentaService.GetByIdAsync(id);
        if (ferramenta == null) return NotFound();
        return Ok(ferramenta.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<FerramentaDto>> Create([FromBody] FerramentaCreateDto dto)
    {
        var ferramenta = dto.ToEntity();
        var created = await _ferramentaService.CriarFerramentaAsync(ferramenta);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }
}