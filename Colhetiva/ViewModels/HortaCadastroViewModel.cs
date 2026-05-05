using System.ComponentModel.DataAnnotations;
using Colhetiva.Core.Enums;

namespace Colhetiva.ViewModels;

public class HortaCadastroViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da horta.")]
    [Display(Name = "Nome")]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Regras")]
    public string Regras { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o responsável (usuário).")]
    [Display(Name = "Responsável")]
    public Guid UsuarioId { get; set; }

    public EnderecoHortaViewModel Endereco { get; set; } = new();

    public List<CanteiroLinhaViewModel> Canteiros { get; set; } = new();

    public List<FerramentaLinhaViewModel> Ferramentas { get; set; } = new();
}

public class EnderecoHortaViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Informe o CEP.")]
    [Display(Name = "CEP")]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a rua.")]
    [Display(Name = "Rua")]
    public string Rua { get; set; } = string.Empty;

    [Display(Name = "Número")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o bairro.")]
    [Display(Name = "Bairro")]
    public string Bairro { get; set; } = string.Empty;

    [Display(Name = "Complemento")]
    public string Complemento { get; set; } = string.Empty;

    [Display(Name = "Latitude")]
    public decimal Latitude { get; set; }

    [Display(Name = "Longitude")]
    public decimal Longitude { get; set; }

    [Required(ErrorMessage = "Selecione a cidade.")]
    [Display(Name = "Cidade")]
    public Guid CidadeId { get; set; }
}

public class CanteiroLinhaViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Identificação")]
    public string? Identificacao { get; set; }

    [Display(Name = "Dimensões")]
    public string? Dimensoes { get; set; }

    [Display(Name = "Status")]
    public StatusCanteiro Status { get; set; } = StatusCanteiro.Disponivel;

    [Display(Name = "Remover")]
    public bool Remover { get; set; }
}

public class FerramentaLinhaViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Nome")]
    public string? Nome { get; set; }

    [Display(Name = "Status")]
    public StatusFerramenta Status { get; set; } = StatusFerramenta.Disponivel;

    [Display(Name = "Remover")]
    public bool Remover { get; set; }
}

public class CanteiroLinhaEditorModel
{
    public int Index { get; set; }
    public CanteiroLinhaViewModel Linha { get; set; } = new();
}

public class FerramentaLinhaEditorModel
{
    public int Index { get; set; }
    public FerramentaLinhaViewModel Linha { get; set; } = new();
}
