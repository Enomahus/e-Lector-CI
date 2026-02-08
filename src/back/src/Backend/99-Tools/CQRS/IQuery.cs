using MediatR;

namespace Tools.CQRS;

public class IQuery<T> : IRequest<T>
{
}
