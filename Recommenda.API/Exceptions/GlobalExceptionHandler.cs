using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Recommenda.Domain.Exceptions;

namespace Recommenda.API.Exceptions;

/// <summary>
/// Tratamento centralizado de excecoes nao capturadas.
/// Retorna respostas no padrao RFC 7807 (application/problem+json).
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Excecao nao tratada: {Message}", exception.Message);

        var (statusCode, title, detail) = MapException(exception, environment);

        httpContext.Response.StatusCode  = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type     = "about:blank",
            Title    = title,
            Status   = statusCode,
            Detail   = detail,
            Instance = httpContext.Request.Path
        };

        if (environment.IsDevelopment())
        {
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        }

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    // -----------------------------------------------------------------------
    // Mapeamento excecao -> status HTTP
    // -----------------------------------------------------------------------
    private static (int StatusCode, string Title, string? Detail) MapException(
        Exception exception,
        IHostEnvironment environment)
    {
        return exception switch
        {
            // 400 — argumentos invalidos
            ArgumentNullException e =>
                (StatusCodes.Status400BadRequest, "Requisicao invalida", e.Message),

            ArgumentException e =>
                (StatusCodes.Status400BadRequest, "Requisicao invalida", e.Message),

            // 400 — violacao de regra de negocio generica
            DomainException e when e.GetType() == typeof(DomainException) =>
                (StatusCodes.Status400BadRequest, "Nao foi possivel concluir a operacao", e.Message),

            // 404 — recurso nao encontrado
            ResourceNotFoundException e =>
                (StatusCodes.Status404NotFound, "Recurso nao encontrado", e.Message),

            KeyNotFoundException e =>
                (StatusCodes.Status404NotFound, "Recurso nao encontrado", e.Message),

            // 409 — conflito de estado (ex.: avaliacao duplicada)
            ConflictException e =>
                (StatusCodes.Status409Conflict, "Conflito", e.Message),

            // 400 — operacao invalida
            InvalidOperationException e =>
                (StatusCodes.Status400BadRequest, "Nao foi possivel concluir a operacao", e.Message),

            // 401 — acesso nao autorizado
            UnauthorizedAccessException e =>
                (StatusCodes.Status401Unauthorized, "Nao autorizado", e.Message),

            // 500 — qualquer outra excecao nao mapeada
            _ => MapUnhandled(environment, exception)
        };
    }

    private static (int StatusCode, string Title, string? Detail) MapUnhandled(
        IHostEnvironment environment,
        Exception exception)
    {
        if (environment.IsDevelopment())
        {
            return (
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor",
                exception.ToString());
        }

        return (
            StatusCodes.Status500InternalServerError,
            "Erro interno do servidor",
            "Ocorreu um erro inesperado. Tente novamente mais tarde.");
    }
}
