using System.Diagnostics.CodeAnalysis;
using Tools.Exceptions;
using Tools.Exceptions.Errors;

namespace Application.Exceptions;

[ExcludeFromCodeCoverage]
public class MissingRequiredDataException : AppException
{
    public MissingRequiredDataException(string entityName, string field, object? key = null)
        : base(
            ErrorCode.MissingData,
            ErrorKind.RequestData,
            $"Entity {entityName}({key}) is missing required field {field}.",
            null,
            new Dictionary<string, string>
            {
                { "type", entityName },
                { "id", key?.ToString() ?? "" },
            }
        ) { }
}
