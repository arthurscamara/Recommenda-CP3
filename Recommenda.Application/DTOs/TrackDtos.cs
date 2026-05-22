using Recommenda.Domain.Entities;

namespace Recommenda.Application.DTOs;

/// <summary>
/// Payload para criacao de uma faixa musical.
/// </summary>
/// <param name="Title">Titulo da faixa.</param>
/// <param name="DurationSeconds">Duracao em segundos (deve ser maior que zero).</param>
/// <param name="TrackNumber">Numero da faixa no album (deve ser maior que zero).</param>
/// <param name="AlbumId">Identificador do album ao qual a faixa pertence.</param>
public record TrackRequest(string Title, int DurationSeconds, int TrackNumber, Guid AlbumId)
{
    public Track ToDomain() => new(Title, DurationSeconds, TrackNumber, AlbumId);
}

/// <summary>
/// Representacao de uma faixa retornada pela API.
/// </summary>
/// <param name="Id">Identificador unico.</param>
/// <param name="Title">Titulo da faixa.</param>
/// <param name="Duration">Duracao formatada (mm:ss).</param>
/// <param name="TrackNumber">Numero da faixa no album.</param>
/// <param name="AlbumId">Identificador do album.</param>
public record TrackResponse(Guid Id, string Title, string Duration, int TrackNumber, Guid AlbumId)
{
    public static TrackResponse FromDomain(Track t) =>
        new(t.Id, t.Title, t.FormattedDuration, t.TrackNumber, t.AlbumId);
}
