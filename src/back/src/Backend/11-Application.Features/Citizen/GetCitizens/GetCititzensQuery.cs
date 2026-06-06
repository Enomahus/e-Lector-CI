using Application.Common.Enums;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.Citizen.GetCitizens
{
    [WithPermission(nameof(AppPermission.GetCitizens))]
    public class GetCititzensQuery : IRequest<Result<List<GetCitizensResponse>>> { }

    public class GetCititzensQueryValidator : AbstractValidator<GetCititzensQuery>
    {
        public GetCititzensQueryValidator() { }
    }

    public class GetCititzensQueryHandler(ReadOnlyDbContext context)
        : IRequestHandler<GetCititzensQuery, Result<List<GetCitizensResponse>>>
    {
        public async Task<Result<List<GetCitizensResponse>>> Handle(
            GetCititzensQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var citizenDaos = await context.Citizens.AsNoTracking().ToListAsync(cancellationToken);

            return Result<List<GetCitizensResponse>>.From([
                .. citizenDaos.Select(GetCitizensResponse.FromDao),
            ]);
        }
    }
}
