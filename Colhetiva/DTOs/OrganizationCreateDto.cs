using System.ComponentModel.DataAnnotations;

namespace Colhetiva.DTOs
{
    public class OrganizationCreateDto
    {
        [Required(ErrorMessage = "Informe o nome da organização.")]
        [Display(Name = "Nome da Organização")]
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

        [Required(ErrorMessage = "Informe o telefone de contato.")]
        [Phone(ErrorMessage = "Telefone inválido.")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o tipo de organização.")]
        [Display(Name = "Tipo de Organização")]
        public string TipoOrganizacao { get; set; } = string.Empty;

        public EnderecoCreateDto Endereco { get; set; } = new EnderecoCreateDto();
    }
}