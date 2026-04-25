using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadosController : ControllerBase
{
    private readonly IEstadoService _estadoService;

    public EstadosController(IEstadoService estadoService)
    {
        _estadoService = estadoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EstadoDto>>> GetAll()
    {
        var estados = await _estadoService.GetAllAsync();
        return Ok(estados.Select(e => e.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EstadoDto>> GetById(Guid id)
    {
        var estado = await _estadoService.GetByIdAsync(id);
        if (estado == null) return NotFound();
        return Ok(estado.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<EstadoDto>> Create([FromBody] EstadoCreateDto dto)
    {
        var estado = dto.ToEntity();
        var created = await _estadoService.CriarEstadoAsync(estado);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }
}