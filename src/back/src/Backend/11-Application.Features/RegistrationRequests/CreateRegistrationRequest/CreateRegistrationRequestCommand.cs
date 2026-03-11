using Application.Models;
using MediatR;

namespace Application.Features.RegistrationRequests.CreateRegistrationRequest
{
    public class CreateRegistrationRequestCommand :  IRequest<Result<Guid>>
    {
    }
}
