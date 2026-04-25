using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CidadesController : ControllerBase
{
    private readonly ICidadeService _cidadeService;

    public CidadesController(ICidadeService cidadeService)
    {
        _cidadeService = cidadeService;
    }

    [HttpGet("estado/{estadoId:guid}")]
    public async Task<ActionResult<List<CidadeDto>>> GetByEstado(Guid estadoId)
    {
        var cidades = await _cidadeService.ProcurarPorUF(estadoId);
        return Ok(cidades.Select(c => c.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<CidadeDto>> Create([FromBody] CidadeCreateDto dto)
    {
        var cidade = dto.ToEntity();
        var created = await _cidadeService.Salvar(cidade);
        return Ok(created.ToDto());
    }
}