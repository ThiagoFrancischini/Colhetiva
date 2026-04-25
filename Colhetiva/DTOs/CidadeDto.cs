namespace Colhetiva.DTOs;

public class CidadeDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid EstadoId { get; set; }
}

public class CidadeCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public Guid EstadoId { get; set; }
}