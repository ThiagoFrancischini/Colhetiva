using Colhetiva.Core.Enums;
using System;
using System.Collections.Generic;

namespace Colhetiva.Core.Input
{
    public class HortaCadastroInput
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Regras { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public EnderecoHortaInput Endereco { get; set; } = new();
        public List<CanteiroLinhaInput> Canteiros { get; set; } = new();
        public List<FerramentaLinhaInput> Ferramentas { get; set; } = new();
    }

    public class EnderecoHortaInput
    {
        public Guid Id { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid CidadeId { get; set; }
    }

    public class CanteiroLinhaInput
    {
        public Guid Id { get; set; }
        public string Identificacao { get; set; } = string.Empty;
        public string Dimensoes { get; set; } = string.Empty;
        public StatusCanteiro Status { get; set; } = StatusCanteiro.Disponivel;
        public bool Remover { get; set; }
    }

    public class FerramentaLinhaInput
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public StatusFerramenta Status { get; set; } = StatusFerramenta.Disponivel;
        public bool Remover { get; set; }
    }
}
