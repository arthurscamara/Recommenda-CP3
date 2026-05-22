using Recommenda.Domain.Entities;

namespace Recommenda.Application.DTOs;

/// <summary>
/// Payload para criacao de uma avaliacao de album.
/// </summary>
/// <param name="UserId">Identificador do usuario que avalia.</param>
/// <param name="AlbumId">Identificador do album avaliado.</param>
/// <param name="Score">Nota de 1 a 5.</param>
/// <param name="Comment">Comentario opcional.</param>
public record AlbumRatingRequest(Guid UserId, Guid AlbumId, int Score, string? Comment = null)
{
    public AlbumRating ToDomain() => new(UserId, AlbumId, Score, Comment);
}

/// <summary>
/// Representacao de uma avaliacao de album retornada pela API.
/// </summary>
/// <param name="Id">Identificador unico.</param>
/// <param name="UserId">Identificador do usuario.</param>
/// <param name="AlbumId">Identificador do album.</param>
/// <param name="Score">Nota de 1 a 5.</param>
/// <param name="Comment">Comentario da avaliacao.</param>
public record AlbumRatingResponse(Guid Id, Guid UserId, Guid AlbumId, int Score, string? Comment)
{
    public static AlbumRatingResponse FromDomain(AlbumRating r) =>
        new(r.Id, r.UserId, r.AlbumId, r.Score, r.Comment);
}
