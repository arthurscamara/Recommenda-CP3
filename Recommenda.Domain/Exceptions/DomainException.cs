namespace Recommenda.Domain.Exceptions;

/// <summary>
/// Excecao base para violacoes de regras de negocio no dominio musical.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
