using Colhetiva.Core.Entities;
using Colhetiva.DTOs;

namespace Colhetiva.Mappings;

public static class EntityMapping
{
    public static FerramentaDto ToDto(this Ferramenta entity) => new()
    {
        Id = entity.Id,
        Nome = entity.Nome,
        HortaId = entity.HortaId,
        Status = entity.Status
    };

    public static Ferramenta ToEntity(this FerramentaDto dto) => new()
    {
        Id = dto.Id,
        Nome = dto.Nome,
        HortaId = dto.HortaId,
        Status = dto.Status
    };

    public static Ferramenta ToEntity(this FerramentaCreateDto dto) => new()
    {
        Nome = dto.Nome,
        HortaId = dto.HortaId,
        Status = Core.Enums.StatusFerramenta.Disponivel
    };

    public static EmprestimoDto ToDto(this Emprestimo entity) => new()
    {
        Id = entity.Id,
        DataRetirada = entity.DataRetirada,
        DataDevolucao = entity.DataDevolucao,
        UsuarioId = entity.UsuarioId,
        FerramentaId = entity.FerramentaId
    };

    public static Emprestimo ToEntity(this EmprestimoCreateDto dto) => new()
    {
        UsuarioId = dto.UsuarioId,
        FerramentaId = dto.FerramentaId
    };

    public static SolicitacaoDto ToDto(this Solicitacao entity) => new()
    {
        Id = entity.Id,
        DataPedido = entity.DataPedido,
        Status = entity.Status,
        UsuarioId = entity.UsuarioId,
        CanteiroId = entity.CanteiroId
    };

    public static Solicitacao ToEntity(this SolicitacaoCreateDto dto) => new()
    {
        UsuarioId = dto.UsuarioId,
        CanteiroId = dto.CanteiroId,
        Status = Core.Enums.StatusSolicitacao.Pendente
    };

    public static CanteiroDto ToDto(this Canteiro entity) => new()
    {
        Id = entity.Id,
        Identificacao = entity.Identificacao,
        Dimensoes = entity.Dimensoes,
        HortaId = entity.HortaId,
        Status = entity.Status
    };

    public static Canteiro ToEntity(this CanteiroCreateDto dto) => new()
    {
        Identificacao = dto.Identificacao,
        Dimensoes = dto.Dimensoes,
        HortaId = dto.HortaId,
        Status = Core.Enums.StatusCanteiro.Disponivel
    };

    public static HortaDto ToDto(this Horta entity) => new()
    {
        Id = entity.Id,
        Nome = entity.Nome,
        Regras = entity.Regras,
        EnderecoId = entity.EnderecoId,
        UsuarioId = entity.UsuarioId
    };

    public static Horta ToEntity(this HortaCreateDto dto) => new()
    {
        Nome = dto.Nome,
        Regras = dto.Regras,
        EnderecoId = dto.EnderecoId,
        UsuarioId = dto.UsuarioId
    };

    public static UsuarioDto ToDto(this Usuario entity) => new()
    {
        Id = entity.Id,
        Nome = entity.Nome,
        CPF = entity.CPF,
        Email = entity.Email,
        EnderecoId = entity.EnderecoId
    };

    public static Usuario ToEntity(this UsuarioCreateDto dto) => new()
    {
        Nome = dto.Nome,
        CPF = dto.CPF,
        Email = dto.Email,
        Password = dto.Password,
        EnderecoId = dto.EnderecoId
    };

    public static EnderecoDto ToDto(this Endereco entity) => new()
    {
        Id = entity.Id,
        Cep = entity.Cep,
        Rua = entity.Rua,
        Numero = entity.Numero,
        Bairro = entity.Bairro,
        CidadeId = entity.CidadeId
    };

    public static Endereco ToEntity(this EnderecoCreateDto dto) => new()
    {
        Cep = dto.Cep,
        Rua = dto.Rua,
        Numero = dto.Numero,
        Bairro = dto.Bairro,
        CidadeId = dto.CidadeId
    };

    public static EstadoDto ToDto(this Estado entity) => new()
    {
        Id = entity.Id,
        Nome = entity.Nome,
        Sigla = entity.Sigla
    };

    public static Estado ToEntity(this EstadoCreateDto dto) => new()
    {
        Nome = dto.Nome,
        Sigla = dto.Sigla
    };

    public static CidadeDto ToDto(this Cidade entity) => new()
    {
        Id = entity.Id,
        Nome = entity.Nome,
        EstadoId = entity.EstadoId
    };

    public static Cidade ToEntity(this CidadeCreateDto dto) => new()
    {
        Nome = dto.Nome,
        EstadoId = dto.EstadoId
    };
}