using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HortasController : ControllerBase
{
    private readonly IHortaService _hortaService;

    public HortasController(IHortaService hortaService)
    {
        _hortaService = hortaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<HortaDto>>> GetAll()
    {
        var hortas = await _hortaService.GetAllAsync();
        return Ok(hortas.Select(h => h.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HortaDto>> GetById(Guid id)
    {
        var horta = await _hortaService.GetByIdAsync(id);
        if (horta == null) return NotFound();
        return Ok(horta.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<HortaDto>> Create([FromBody] HortaCreateDto dto)
    {
        var horta = dto.ToEntity();
        var created = await _hortaService.CriarHortaAsync(horta);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }
}