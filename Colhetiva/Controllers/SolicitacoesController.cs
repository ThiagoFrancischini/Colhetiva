using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitacoesController : ControllerBase
{
    private readonly ISolicitacaoService _solicitacaoService;

    public SolicitacoesController(ISolicitacaoService solicitacaoService)
    {
        _solicitacaoService = solicitacaoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SolicitacaoDto>>> GetAll()
    {
        var solicitacoes = await _solicitacaoService.GetAllAsync();
        return Ok(solicitacoes.Select(s => s.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SolicitacaoDto>> GetById(Guid id)
    {
        var solicitacao = await _solicitacaoService.GetByIdAsync(id);
        if (solicitacao == null) return NotFound();
        return Ok(solicitacao.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<SolicitacaoDto>> Create([FromBody] SolicitacaoCreateDto dto)
    {
        var solicitacao = dto.ToEntity();
        var created = await _solicitacaoService.CriarSolicitacaoAsync(solicitacao);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }
}