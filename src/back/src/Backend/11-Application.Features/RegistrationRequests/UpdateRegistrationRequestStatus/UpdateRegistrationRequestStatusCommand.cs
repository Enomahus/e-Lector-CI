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
using Tools.Constants;
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
        //public long? PollingStationId { get; set; }
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

            //RuleFor(r => r.PollingStationId)
            //    .NotEmpty()
            //    .When(r => r.NewStatus == RegistrationStatus.Approved)
            //    .WithMessage(ValidationErrorCode.Required.ToString())
            //    .MustAsync(
            //        async (id, token) =>
            //        {
            //            if (!id.HasValue)
            //                return false;
            //            return await context.PollingStations.AnyAsync(ps => ps.Id == id.Value, token);
            //        }
            //    )
            //    .WithMessage(ValidationErrorCode.PollingStationMustExist.ToString());
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
                    //command.PollingStationId!.Value,
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
            //long pollingSationId,
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

            var pollingStation = await GetOrCreateAvailablePollingStationAsync(dao.ConstituencyId, token);

            string voterNumber = await referenceGeneratorService.GenerateElectorNumberAsync(
                context,
                pollingStation.Id,
                token
            );

            // Création du profil Électeur
            var elector = new ElectorDao
            {
                Id = dao.CitizenId, // Relation 1:1 avec Citizen
                VoterRegistrationNumber = voterNumber,
                RegistrationDate = now,
                Status = ElectorStatus.Active,
                PollingStationId = pollingStation.Id,
                CreatedAt = now,
                ModifiedAt = now,
            };

            dao.Status = RegistrationStatus.Approved;
            context.Electors.Add(elector);
        }

        private async Task<PollingStationDao> GetOrCreateAvailablePollingStationAsync(
            long constituencyId,
            CancellationToken cancellationToken
        )
        {
            const int maxElectorsPerStation = AppConstants.MAX_ELECTORS_PER_STATION;

            var constituency =
                await context
                    .Constituencies.Include(ps => ps.PollingStations)
                        .ThenInclude(pse => pse.Electors)
                    .FirstOrDefaultAsync(c => c.Id == constituencyId, cancellationToken)
                ?? throw new NotFoundException(nameof(ConstituencyDao), constituencyId);

            if (constituency.Level != LocationLevel.VotingLocation)
            {
                throw new InvalidOperationException(
                    "Les bureaux de vôte ne peuvent être créés que pour une circonscription de niveau LIEU DE VOTE"
                );
            }

            var availableStation = constituency.PollingStations.FirstOrDefault(ps =>
                ps.Electors.Count < maxElectorsPerStation
            );

            if (availableStation is not null)
            {
                return availableStation;
            }

            int nextNumber =
                constituency.PollingStations.Count == 0
                    ? 1
                    : constituency
                        .PollingStations.Select(ps => int.TryParse(ps.StationNumber, out var n) ? n : 0)
                        .Max() + 1;

            var newStation = new PollingStationDao
            {
                StationNumber = nextNumber.ToString("D2"),
                Wording = "",
                ConstituencyId = constituencyId,
                CreatedAt = timeProvider.GetUtcNow(),
                ModifiedAt = timeProvider.GetUtcNow(),
            };

            context.PollingStations.Add(newStation);
            await context.SaveChangesAsync(cancellationToken);

            return newStation;
        }
    }
}
