using System.ComponentModel.DataAnnotations;

namespace Colhetiva.DTOs;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid EnderecoId { get; set; }
}

public class UsuarioCreateDto
{

    [Required(ErrorMessage = "Informe o nome completo.")]
    [Display(Name = "Nome Completo")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "As senhas não conferem.")]
    [Display(Name = "Confirmar Senha")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CPF.")]
    [MinLength(11, ErrorMessage = "CPF inválido.")]
    [MaxLength(14, ErrorMessage = "CPF inválido.")]
    [Display(Name = "CPF")]
    public string? CPF { get; set; }
    public Guid EnderecoId { get; set; }
    public EnderecoCreateDto Endereco { get; set; } = new EnderecoCreateDto();
}

public class UsuarioLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}