namespace Recommenda.Domain.Exceptions;

/// <summary>
/// Excecao lancada quando um recurso solicitado nao e encontrado no banco de dados.
/// </summary>
public class ResourceNotFoundException : DomainException
{
    public ResourceNotFoundException(string resourceName, Guid id)
        : base($"{resourceName} com id '{id}' nao foi encontrado.") { }

    public ResourceNotFoundException(string message) : base(message) { }
}
