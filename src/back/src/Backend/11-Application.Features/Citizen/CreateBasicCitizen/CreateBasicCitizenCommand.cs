using Application.Common.Enums;
using Application.Features.Common.Citizen;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.Citizen.CreateBasicCitizen
{
    [WithPermission(nameof(AppPermission.CreateBasicCitizen))]
    public class CreateBasicCitizenCommand : BasicCitizenModel, IRequest<Result<Guid>> { }

    public class CreateBasicCitizenCommandValidator : AbstractValidator<CreateBasicCitizenCommand>
    {
        public CreateBasicCitizenCommandValidator(TimeProvider timeProvider)
        {
            RuleFor(v => v.FirstName).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
            RuleFor(v => v.LastName).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
            RuleFor(v => v.BirthDate)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .Must(date => date <= timeProvider.GetUtcNow().DateTime.AddYears(-18))
                .WithMessage(ValidationErrorCode.InvalidDate.ToString());

            RuleFor(v => v.BirthPlace).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
            RuleFor(v => v.Nationality).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
        }
    }

    public class CreateBasicCitizenCommandHandler(WritableDbContext context)
        : IRequestHandler<CreateBasicCitizenCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            CreateBasicCitizenCommand command,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var newEntity = new CitizenDao
            {
                Gender = command.Gender,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate!.Value,
                BirthPlace = command.BirthPlace,
                Nationality = command.Nationality,
            };
            context.Citizens.Add(newEntity);
            await context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.From(newEntity.Id);
        }
    }
}
