namespace Colhetiva.DTOs;

public class HortaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Regras { get; set; } = string.Empty;
    public Guid EnderecoId { get; set; }
    public Guid UsuarioId { get; set; }
}

public class HortaCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Regras { get; set; } = string.Empty;
    public Guid EnderecoId { get; set; }
    public Guid UsuarioId { get; set; }
}