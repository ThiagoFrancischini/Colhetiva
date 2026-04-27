using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Informe o CEP.")]
    [Display(Name = "CEP")]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a rua.")]
    [Display(Name = "Rua")]
    public string Rua { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o bairro.")]
    [Display(Name = "Bairro")]
    public string Bairro { get; set; } = string.Empty;

    [Display(Name = "Número")]
    public string? Numero { get; set; }

    [Display(Name = "Complemento")]
    public string? Complemento { get; set; }

    [Required(ErrorMessage = "Selecione a cidade.")]
    [Display(Name = "Cidade")]
    public Guid CidadeId { get; set; }
}