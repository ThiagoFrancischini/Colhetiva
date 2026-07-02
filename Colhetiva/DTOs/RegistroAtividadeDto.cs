namespace Colhetiva.DTOs;

public record CriarAtividadeDto(
    Guid HortaId,
    Guid? CanteiroId,
    DateTime DataHora,
    string Atividade,
    string? Observacoes,
    string? FotoUrl
);

public record ItemCalendarioDto(
    Guid Id,
    DateTime DataHora,
    string Atividade,
    string NomeHorta,
    string? NomeCanteiro
);

public record TimelineHortaDto(
    Guid Id,
    DateTime DataHora,
    string Atividade,
    string? Observacoes,
    string? FotoUrl,
    string NomeUsuario,
    string? FotoPerfilUsuario,
    string? NomeCanteiro
);
