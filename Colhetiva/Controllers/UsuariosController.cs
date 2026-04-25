using Microsoft.AspNetCore.Mvc;
using Colhetiva.Core.Interfaces.Service;
using Colhetiva.DTOs;
using Colhetiva.Mappings;

namespace Colhetiva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> GetById(Guid id)
    {
        var usuario = await _usuarioService.GetById(id);
        if (usuario == null) return NotFound();
        return Ok(usuario.ToDto());
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UsuarioDto>> GetByEmail(string email)
    {
        var usuario = await _usuarioService.GetByEmail(email);
        if (usuario == null) return NotFound();
        return Ok(usuario.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioDto>>> GetAll()
    {
        var usuarios = await _usuarioService.GetAll();
        return Ok(usuarios.Select(u => u.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Create([FromBody] UsuarioCreateDto dto)
    {
        var usuario = dto.ToEntity();
        var created = await _usuarioService.Salvar(usuario);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPost("login")]
    public async Task<ActionResult<UsuarioDto>> Login([FromBody] UsuarioLoginDto dto)
    {
        var usuario = await _usuarioService.Autenticar(dto.Email, dto.Password);
        if (usuario == null) return Unauthorized();
        return Ok(usuario.ToDto());
    }
}