using System.Diagnostics.CodeAnalysis;
using Tools.Exceptions;
using Tools.Exceptions.Errors;

namespace Application.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidStatusException : AppException
{
    public InvalidStatusException(string objectType, string? status)
        : base(
            ErrorCode.InvalidStatus,
            ErrorKind.RequestData,
            $"Status ({status}) is not allowed for entity {objectType}.",
            null,
            new Dictionary<string, string> { { "type", objectType }, { "status", status ?? "" } }
        ) { }
}
