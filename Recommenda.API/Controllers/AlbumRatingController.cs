using Microsoft.AspNetCore.Mvc;
using Recommenda.Application.DTOs;
using Recommenda.Application.Services;
using Recommenda.Domain.Exceptions;

namespace Recommenda.API.Controllers;

/// <summary>
/// Gerenciamento de avaliacoes de albuns por usuarios.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AlbumRatingController(
    IAlbumRatingRepository ratingRepository,
    IAlbumRepository albumRepository,
    IUserRepository userRepository) : ControllerBase
{
    /// <summary>
    /// Lista todas as avaliacoes de um album especifico.
    /// </summary>
    /// <param name="albumId">Identificador do album.</param>
    /// <returns>Lista de avaliacoes do album.</returns>
    [HttpGet("album/{albumId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<AlbumRatingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetByAlbum(Guid albumId)
    {
        if (albumRepository.GetById(albumId) is null)
            throw new ResourceNotFoundException("Album", albumId);

        var ratings = ratingRepository.GetByAlbum(albumId).Select(AlbumRatingResponse.FromDomain);
        return Ok(ratings);
    }

    /// <summary>
    /// Lista todas as avaliacoes feitas por um usuario especifico.
    /// </summary>
    /// <param name="userId">Identificador do usuario.</param>
    /// <returns>Lista de avaliacoes do usuario.</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<AlbumRatingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetByUser(Guid userId)
    {
        if (userRepository.GetById(userId) is null)
            throw new ResourceNotFoundException("Usuario", userId);

        var ratings = ratingRepository.GetByUser(userId).Select(AlbumRatingResponse.FromDomain);
        return Ok(ratings);
    }

    /// <summary>
    /// Registra a avaliacao de um usuario sobre um album.
    /// </summary>
    /// <remarks>
    /// Cada usuario pode avaliar um album apenas uma vez. Score deve estar entre 1 e 5.
    ///
    ///     POST /api/albumrating
    ///     {
    ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "albumId": "7fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "score": 5,
    ///         "comment": "Album incrivel!"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">Dados da avaliacao.</param>
    /// <returns>Avaliacao criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AlbumRatingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] AlbumRatingRequest request)
    {
        if (userRepository.GetById(request.UserId) is null)
            throw new ResourceNotFoundException("Usuario", request.UserId);

        if (albumRepository.GetById(request.AlbumId) is null)
            throw new ResourceNotFoundException("Album", request.AlbumId);

        if (ratingRepository.Exists(request.UserId, request.AlbumId))
            throw new ConflictException("Usuario ja avaliou este album.");

        var rating = ratingRepository.Create(request.ToDomain());
        return CreatedAtAction(null, AlbumRatingResponse.FromDomain(rating));
    }
}
