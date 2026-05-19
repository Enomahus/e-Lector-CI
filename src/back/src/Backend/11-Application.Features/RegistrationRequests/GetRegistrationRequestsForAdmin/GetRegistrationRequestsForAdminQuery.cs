using Application.Common.Enums;
using Application.Features.Common;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Pcea.Core.Net.Authorization.Application.Attributes;

namespace Application.Features.RegistrationRequests.GetRegistrationRequestsForAdmin
{
    [WithPermission([nameof(AppPermission.GetRegistrationRequestsFormAdmin)])]
    public class GetRegistrationRequestsForAdminQuery
        : IRequest<Result<PagedList<GetRegistrationRequestsForAdminResponse>>>
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetRegistrationRequestsForAdminQueryValidator
        : AbstractValidator<GetRegistrationRequestsForAdminQuery>
    {
        public GetRegistrationRequestsForAdminQueryValidator() { }
    }

    public class GetRegistrationRequestsForAdminQueryHandler(ReadOnlyDbContext context)
        : IRequestHandler<
            GetRegistrationRequestsForAdminQuery,
            Result<PagedList<GetRegistrationRequestsForAdminResponse>>
        >
    {
        public Task<Result<PagedList<GetRegistrationRequestsForAdminResponse>>> Handle(
            GetRegistrationRequestsForAdminQuery request,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }
    }
}
