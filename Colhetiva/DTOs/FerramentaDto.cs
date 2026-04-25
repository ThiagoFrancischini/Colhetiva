using Colhetiva.Core.Enums;

namespace Colhetiva.DTOs;

public class FerramentaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid HortaId { get; set; }
    public StatusFerramenta Status { get; set; }
}

public class FerramentaCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public Guid HortaId { get; set; }
}