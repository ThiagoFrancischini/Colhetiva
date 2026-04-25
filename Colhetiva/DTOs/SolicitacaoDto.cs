using Colhetiva.Core.Enums;

namespace Colhetiva.DTOs;

public class SolicitacaoDto
{
    public Guid Id { get; set; }
    public DateTime DataPedido { get; set; }
    public StatusSolicitacao Status { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid CanteiroId { get; set; }
}

public class SolicitacaoCreateDto
{
    public Guid UsuarioId { get; set; }
    public Guid CanteiroId { get; set; }
}