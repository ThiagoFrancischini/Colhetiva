namespace Colhetiva.DTOs;

public class EstadoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
}

public class EstadoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
}