namespace Recommenda.Domain.Exceptions;

/// <summary>
/// Excecao lancada quando uma operacao viola uma restricao de unicidade ou conflito de estado.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}
