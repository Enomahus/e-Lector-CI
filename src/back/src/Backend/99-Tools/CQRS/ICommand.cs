using MediatR;

namespace Tools.CQRS;

public class ICommand<T> : IRequest<T>
{
}
