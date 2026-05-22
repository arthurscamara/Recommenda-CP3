using Recommenda.Domain.Entities;

namespace Recommenda.Application.DTOs;

/// <summary>
/// Payload para criacao ou atualizacao de um genero musical.
/// </summary>
/// <param name="Name">Nome do genero (ex.: Rock, Jazz).</param>
/// <param name="Description">Descricao opcional do genero.</param>
public record GenreRequest(string Name, string Description = "")
{
    public Genre ToDomain() => new(Name, Description);
}

/// <summary>
/// Representacao resumida de um genero musical retornado pela API.
/// </summary>
/// <param name="Id">Identificador unico.</param>
/// <param name="Name">Nome do genero.</param>
/// <param name="Description">Descricao do genero.</param>
public record GenreResponse(Guid Id, string Name, string Description)
{
    public static GenreResponse FromDomain(Genre g) => new(g.Id, g.Name, g.Description);
}
