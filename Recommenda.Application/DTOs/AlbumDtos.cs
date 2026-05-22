using Recommenda.Domain.Entities;

namespace Recommenda.Application.DTOs;

/// <summary>
/// Payload para criacao de um album.
/// </summary>
/// <param name="Title">Titulo do album.</param>
/// <param name="ReleaseDate">Data de lancamento.</param>
/// <param name="ArtistId">Identificador do artista dono do album.</param>
/// <param name="CoverUrl">URL da capa (opcional).</param>
public record AlbumRequest(
    string Title,
    DateTime ReleaseDate,
    Guid ArtistId,
    string CoverUrl = ""
)
{
    public Album ToDomain() => new(Title, ReleaseDate, ArtistId, CoverUrl);
}

/// <summary>
/// Representacao de um album retornado pela API.
/// </summary>
/// <param name="Id">Identificador unico.</param>
/// <param name="Title">Titulo do album.</param>
/// <param name="ReleaseDate">Data de lancamento.</param>
/// <param name="ArtistId">Identificador do artista.</param>
/// <param name="CoverUrl">URL da capa.</param>
public record AlbumResponse(Guid Id, string Title, DateTime ReleaseDate, Guid ArtistId, string CoverUrl)
{
    public static AlbumResponse FromDomain(Album a) =>
        new(a.Id, a.Title, a.ReleaseDate, a.ArtistId, a.CoverUrl);
}
