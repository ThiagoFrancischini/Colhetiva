using System.ComponentModel.DataAnnotations;

namespace Colhetiva.DTOs
{
    public class ProfileViewDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Informe o nome.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "CPF")]
        public string CPF { get; set; } = string.Empty;

        [Display(Name = "Função")]
        public string Role { get; set; } = string.Empty;

        public EnderecoDto Endereco { get; set; } = new EnderecoDto();
    }

    public class ProfileUpdateDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();
    }
}