using Tools.Exceptions;
using Tools.Exceptions.Errors;

namespace Application.Exceptions;

public class IncorrectEntityTypeException : AppException
{
    public IncorrectEntityTypeException(string expectedType, string? foundType)
        : base(
            ErrorCode.IncorrectEntityTypeLinked,
            ErrorKind.Technical,
            $"Entity has type {foundType} instead of {expectedType}"
        ) { }
}
