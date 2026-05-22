using Recommenda.Domain.Entities;

namespace Recommenda.Application.DTOs;

/// <summary>
/// Payload para criacao de um artista ou banda.
/// </summary>
/// <param name="Name">Nome do artista ou banda.</param>
/// <param name="Bio">Biografia resumida.</param>
/// <param name="Country">Pais de origem (ex.: Brasil, EUA).</param>
public record ArtistRequest(string Name, string Bio, string Country)
{
    public Artist ToDomain() => new(Name, Bio, Country);
}

/// <summary>
/// Representacao de um artista retornado pela API.
/// </summary>
/// <param name="Id">Identificador unico.</param>
/// <param name="Name">Nome do artista ou banda.</param>
/// <param name="Country">Pais de origem.</param>
/// <param name="Bio">Biografia resumida.</param>
public record ArtistResponse(Guid Id, string Name, string Country, string Bio)
{
    public static ArtistResponse FromDomain(Artist a) => new(a.Id, a.Name, a.Country, a.Bio);
}
