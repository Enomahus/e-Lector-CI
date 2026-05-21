using Application.Common.Enums;
using Application.Exceptions;
using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.UpdateRegistrationRequestStatus
{
    [WithPermission(nameof(AppPermission.TriggerActionOnRegistrationRequest))]
    public class UpdateRegistrationRequestStatusCommand
        : IRequest<Result<UpdateRegistrationRequestStatusResponse>>
    {
        public Guid RegistrationRequestId { get; set; }
        public RegistrationStatus NewStatus { get; set; }
        public string? ReasonForRejection { get; set; }
        public long? PollingStationId { get; set; }
    }

    public class UpdateRegistrationRequestStatusCommandValidator
        : AbstractValidator<UpdateRegistrationRequestStatusCommand>
    {
        public UpdateRegistrationRequestStatusCommandValidator(ReadOnlyDbContext context)
        {
            RuleFor(r => r.RegistrationRequestId)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(r => r.NewStatus).IsInEnum();

            RuleFor(r => r.ReasonForRejection)
                .NotEmpty()
                .When(r => r.NewStatus == RegistrationStatus.Rejected)
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MaximumLength(500);

            RuleFor(r => r.PollingStationId)
                .NotEmpty()
                .When(r => r.NewStatus == RegistrationStatus.Approved)
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MustAsync(
                    async (id, token) =>
                    {
                        if (!id.HasValue)
                            return false;
                        return await context.PollingStations.AnyAsync(ps => ps.Id == id.Value, token);
                    }
                )
                .WithMessage(ValidationErrorCode.PollingStationMustExist.ToString());
        }
    }

    public class UpdateRegistrationRequestStatusCommandValidatorHandler(
        WritableDbContext context,
        TimeProvider timeProvider,
        IReferenceGeneratorService referenceGeneratorService,
        ICurrentUserService currentUserService
    )
        : IRequestHandler<
            UpdateRegistrationRequestStatusCommand,
            Result<UpdateRegistrationRequestStatusResponse>
        >
    {
        public async Task<Result<UpdateRegistrationRequestStatusResponse>> Handle(
            UpdateRegistrationRequestStatusCommand command,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog
                .CQRS.Start()
                .AddParameter(command, r => r.RegistrationRequestId);

            var dateNow = timeProvider.GetUtcNow();
            var currentUserId = currentUserService.UserId;

            //var currentUser = await context.Users
            //    .Include(u => u.UserConstituencies)
            //    .ThenInclude(c => c.Constituency)
            //    .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken)
            //    ?? throw new NotFoundException(nameof(UserDao), currentUserId);

            var registrationDao =
                await context
                    .RegistrationRequests.Include(r => r.Citizen)
                    .Include(r => r.Constituency)
                    .FirstOrDefaultAsync(
                        r =>
                            r.Id == command.RegistrationRequestId
                            && r.Status == RegistrationStatus.ToBeProcessed,
                        cancellationToken
                    )
                ?? throw new NotFoundException(nameof(RegistrationRequestDao), command.RegistrationRequestId);

            if (command.NewStatus == RegistrationStatus.Approved)
            {
                await ProcessApprovalAsync(
                    registrationDao,
                    command.PollingStationId!.Value,
                    dateNow,
                    cancellationToken
                );
            }
            else if (command.NewStatus == RegistrationStatus.Rejected)
            {
                registrationDao.Status = RegistrationStatus.Rejected;
                registrationDao.ReasonForRejection = command.ReasonForRejection;
            }

            registrationDao.LastUpdaterId = currentUserId;

            await context.SaveChangesAsync(cancellationToken);

            return Result<UpdateRegistrationRequestStatusResponse>.From(
                UpdateRegistrationRequestStatusResponse.FromDao(registrationDao)
            );
        }

        private async Task ProcessApprovalAsync(
            RegistrationRequestDao dao,
            long pollingSationId,
            DateTimeOffset now,
            CancellationToken token
        )
        {
            // Logique métier : Un citoyen ne peut pas avoir deux profils électeurs actifs
            var alreadyElector = await context.Electors.AnyAsync(
                e => e.Id == dao.CitizenId && e.Status == ElectorStatus.Active,
                token
            );

            if (alreadyElector)
                throw new Exception("Le citoyen est déjà inscrit comme électeur.");

            string voterNumber = await referenceGeneratorService.GenerateElectorNumberAsync(
                context,
                pollingSationId,
                token
            );

            // Création du profil Électeur
            var elector = new ElectorDao
            {
                Id = dao.CitizenId, // Relation 1:1 avec Citizen
                VoterRegistrationNumber = voterNumber,
                RegistrationDate = now,
                Status = ElectorStatus.Active,
                PollingStationId = pollingSationId,
                CreatedAt = now,
                ModifiedAt = now,
            };

            dao.Status = RegistrationStatus.Approved;
            context.Electors.Add(elector);
        }
    }
}
