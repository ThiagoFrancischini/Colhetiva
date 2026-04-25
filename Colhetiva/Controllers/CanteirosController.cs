using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CanteirosController : ControllerBase
{
    private readonly ICanteiroService _canteiroService;

    public CanteirosController(ICanteiroService canteiroService)
    {
        _canteiroService = canteiroService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CanteiroDto>>> GetAll()
    {
        var canteiros = await _canteiroService.GetAllAsync();
        return Ok(canteiros.Select(c => c.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CanteiroDto>> GetById(Guid id)
    {
        var canteiro = await _canteiroService.GetByIdAsync(id);
        if (canteiro == null) return NotFound();
        return Ok(canteiro.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CanteiroDto>> Create([FromBody] CanteiroCreateDto dto)
    {
        var canteiro = dto.ToEntity();
        var created = await _canteiroService.CriarCanteiroAsync(canteiro);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }
}