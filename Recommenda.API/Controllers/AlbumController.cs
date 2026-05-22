using Microsoft.AspNetCore.Mvc;
using Recommenda.Application.DTOs;
using Recommenda.Application.Services;
using Recommenda.Domain.Exceptions;

namespace Recommenda.API.Controllers;

/// <summary>
/// Gerenciamento de albuns de estudio, EPs e coletaneas.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AlbumController(
    IAlbumRepository albumRepository,
    IArtistRepository artistRepository) : ControllerBase
{
    /// <summary>
    /// Lista todos os albuns cadastrados.
    /// </summary>
    /// <returns>Lista de albuns.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlbumResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var albums = albumRepository.GetAll().Select(AlbumResponse.FromDomain);
        return Ok(albums);
    }

    /// <summary>
    /// Busca um album pelo identificador unico.
    /// </summary>
    /// <param name="id">Identificador do album.</param>
    /// <returns>Dados do album encontrado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AlbumResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var album = albumRepository.GetById(id);
        if (album is null)
            throw new ResourceNotFoundException("Album", id);

        return Ok(AlbumResponse.FromDomain(album));
    }

    /// <summary>
    /// Lista todos os albuns de um artista especifico.
    /// </summary>
    /// <param name="artistId">Identificador do artista.</param>
    /// <returns>Lista de albuns do artista.</returns>
    [HttpGet("artist/{artistId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<AlbumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetByArtist(Guid artistId)
    {
        if (artistRepository.GetById(artistId) is null)
            throw new ResourceNotFoundException("Artista", artistId);

        var albums = albumRepository.GetByArtist(artistId).Select(AlbumResponse.FromDomain);
        return Ok(albums);
    }

    /// <summary>
    /// Cria um novo album para um artista existente.
    /// </summary>
    /// <remarks>
    /// Exemplo de corpo:
    ///
    ///     POST /api/album
    ///     {
    ///         "title": "No na Orelha",
    ///         "releaseDate": "2011-07-12T00:00:00",
    ///         "artistId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "coverUrl": "https://example.com/capa.jpg"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">Dados do album a ser criado.</param>
    /// <returns>Album criado com seu identificador gerado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AlbumResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Create([FromBody] AlbumRequest request)
    {
        if (artistRepository.GetById(request.ArtistId) is null)
            throw new ResourceNotFoundException("Artista", request.ArtistId);

        var album = albumRepository.Create(request.ToDomain());
        return CreatedAtAction(nameof(GetById), new { id = album.Id }, AlbumResponse.FromDomain(album));
    }

    /// <summary>
    /// Remove um album pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do album a ser removido.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!albumRepository.Delete(id))
            throw new ResourceNotFoundException("Album", id);

        return NoContent();
    }
}
