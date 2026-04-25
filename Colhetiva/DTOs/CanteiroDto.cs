using Colhetiva.Core.Enums;

namespace Colhetiva.DTOs;

public class CanteiroDto
{
    public Guid Id { get; set; }
    public string Identificacao { get; set; } = string.Empty;
    public string Dimensoes { get; set; } = string.Empty;
    public Guid HortaId { get; set; }
    public StatusCanteiro Status { get; set; }
}

public class CanteiroCreateDto
{
    public string Identificacao { get; set; } = string.Empty;
    public string Dimensoes { get; set; } = string.Empty;
    public Guid HortaId { get; set; }
}