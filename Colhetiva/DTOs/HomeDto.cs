using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;

namespace Colhetiva.ViewModels;

public class HomeDto
{
    public List<HortaCardDto> Hortas { get; set; } = new();
    public string? NomeUsuario { get; set; }
}

public class HortaCardDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Regras { get; set; } = string.Empty;

    // Endereço
    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;

    // Responsável
    public string Responsavel { get; set; } = string.Empty;

    // Estatísticas
    public int TotalCanteiros { get; set; }
    public int CanteirosDisponiveis { get; set; }
    public int TotalFerramentas { get; set; }
    public int FerramentasDisponiveis { get; set; }

    public string EnderecoCompleto =>
        string.IsNullOrWhiteSpace(Numero)
            ? $"{Rua}, {Bairro} — {Cidade}"
            : $"{Rua}, {Numero} — {Bairro} — {Cidade}";

    public bool TemCanteiroDisponivel => CanteirosDisponiveis > 0;

    public static HortaCardDto FromEntity(Horta h) => new()
    {
        Id = h.Id,
        Nome = h.Nome,
        Regras = h.Regras,
        Rua = h.Endereco?.Rua ?? string.Empty,
        Numero = h.Endereco?.Numero ?? string.Empty,
        Bairro = h.Endereco?.Bairro ?? string.Empty,
        Cidade = h.Endereco?.Cidade?.Nome ?? string.Empty,
        Responsavel = h.Usuario?.Nome ?? string.Empty,
        TotalCanteiros = h.Canteiros.Count,
        CanteirosDisponiveis = h.Canteiros.Count(c => c.Status == StatusCanteiro.Disponivel),
        TotalFerramentas = h.Ferramentas.Count,
        FerramentasDisponiveis = h.Ferramentas.Count(f => f.Status == StatusFerramenta.Disponivel),
    };
}
