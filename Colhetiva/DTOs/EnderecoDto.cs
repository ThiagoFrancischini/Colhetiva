namespace Colhetiva.DTOs;

public class EnderecoDto
{
    public Guid Id { get; set; }
    public string Cep { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public Guid CidadeId { get; set; }
}

public class EnderecoCreateDto
{
    public string Cep { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public Guid CidadeId { get; set; }
}