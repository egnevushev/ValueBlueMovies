using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    
    public DomainException(string message, Exception exception) : base(message, exception) { }

    public static void ThrowIfNull<T>([NotNull]T? argument, [CallerArgumentExpression("argument")] string? paramName = default)
    {
        if (argument is null)
        {
            throw new DomainException($"{paramName} can't be null", new NullReferenceException(paramName));
        }    
    }
}