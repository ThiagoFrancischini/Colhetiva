namespace Colhetiva.DTOs;

public class EmprestimoDto
{
    public Guid Id { get; set; }
    public DateTime DataRetirada { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid FerramentaId { get; set; }
}

public class EmprestimoCreateDto
{
    public Guid UsuarioId { get; set; }
    public Guid FerramentaId { get; set; }
}