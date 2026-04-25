using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnderecosController : ControllerBase
{
    private readonly IEnderecoService _enderecoService;

    public EnderecosController(IEnderecoService enderecoService)
    {
        _enderecoService = enderecoService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EnderecoDto>> GetById(Guid id)
    {
        var endereco = await _enderecoService.GetById(id);
        if (endereco == null) return NotFound();
        return Ok(endereco.ToDto());
    }

    [HttpGet("cep/{cep}")]
    public async Task<ActionResult<List<EnderecoDto>>> GetByCep(string cep)
    {
        var enderecos = await _enderecoService.GetByCep(cep);
        return Ok(enderecos.Select(e => e.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<EnderecoDto>> Create([FromBody] EnderecoCreateDto dto)
    {
        var endereco = dto.ToEntity();
        var created = await _enderecoService.Salvar(endereco);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }
}