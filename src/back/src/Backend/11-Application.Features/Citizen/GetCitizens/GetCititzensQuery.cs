using System;
using System.Collections.Generic;
using System.Text;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.Citizen.GetCitizens
{
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

            var citizendao = await context.Citizens.AsNoTracking().ToListAsync(cancellationToken);

            return Result<List<GetCitizensResponse>>.From([
                .. citizendao.Select(GetCitizensResponse.FromDao),
            ]);
        }
    }
}
