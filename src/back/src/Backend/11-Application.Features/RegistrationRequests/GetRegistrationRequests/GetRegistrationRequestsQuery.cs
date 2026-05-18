using Application.Common.Enums;
using Application.Features.Common;
using Application.Features.Common.Citizen;
using Application.Interfaces.Services;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.GetRegistrationRequests
{
    [WithPermission([nameof(AppPermission.GetRegistrationRequests)])]
    public class GetRegistrationRequestsQuery : IRequest<Result<PagedList<GetRegistrationRequestsResponse>>>
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetRegistrationRequestsQueryValidator : AbstractValidator<GetRegistrationRequestsQuery>
    {
        public GetRegistrationRequestsQueryValidator() { }
    }

    public class GetRegistrationRequestsQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider,
        ICurrentUserService currentUserService
    ) : IRequestHandler<GetRegistrationRequestsQuery, Result<PagedList<GetRegistrationRequestsResponse>>>
    {
        public async Task<Result<PagedList<GetRegistrationRequestsResponse>>> Handle(
            GetRegistrationRequestsQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var currentUserId = currentUserService.UserId;

            var registrationRequests = await context
                .RegistrationRequests.Include(r => r.Citizen)
                .Where(rr => rr.AuthorId == currentUserId)
                .Select(r => new GetRegistrationRequestsResponse
                {
                    Id = r.Id,
                    Reference = r.Reference,
                    SubmissionDate = r.SubmissionDate,
                    Status = r.Status,
                    ConstituencyId = r.ConstituencyId,
                    ConstituencyName = r.Constituency.Wording,
                    Comment = r.ReasonForRejection,
                    Citizen = CitizenModel.FromDao(r.Citizen),
                    CanBeDeleted =
                        currentUserId == r.AuthorId && (r.Status == RegistrationStatus.ToBeProcessed),
                })
                .ToListAsync(cancellationToken);

            throw new NotImplementedException();
        }
    }
}
