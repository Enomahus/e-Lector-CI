using Application.Exceptions;
using Application.Interfaces.Services;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.RegistrationRequests.Common
{
    public class ReferenceGeneratorService : IReferenceGeneratorService
    {
        public async Task<string> GenerateElectorNumberAsync(
            WritableDbContext context,
            long pollingStaionId,
            CancellationToken cancellationToken
        )
        {
            // 1. Récupérer le code de la zone (ex: 0034) via le bureau de vote
            var station =
                await context
                    .PollingStations.Include(p => p.Constituency)
                    .FirstOrDefaultAsync(p => p.Id == pollingStaionId, cancellationToken)
                ?? throw new NotFoundException(nameof(PollingStationDao), pollingStaionId);

            string zoneCode = station.Constituency.Code.PadLeft(5, '0');

            // 2. Compter le nombre d'électeurs actuels pour cette zone/bureau
            // pour obtenir le numéro de séquence (ex: 6601)
            //int sequenceNumber =
            //    await context.Electors.CountAsync(
            //        e => e.PollingStationId == pollingStaionId,
            //        cancellationToken
            //    ) + 1;
            var lastElectorNumber = await context
                .Electors.Where(e => e.PollingStationId == pollingStaionId)
                .OrderByDescending(e => e.VoterRegistrationNumber)
                .Select(e => e.VoterRegistrationNumber)
                .FirstOrDefaultAsync(cancellationToken);

            int sequenceNumber = 0;
            if (!string.IsNullOrEmpty(lastElectorNumber))
            {
                var seq = lastElectorNumber.Split(' ')[2];
                _ = int.TryParse(seq, out sequenceNumber);
            }

            string sequenceStr = sequenceNumber.ToString().PadLeft(6, '0');

            // 3. Calculer une clé de contrôle (Modulo 97) pour l'intégrité (ex: 11)
            // On concatène la zone et la séquence pour le calcul
            long rawNumber = long.Parse($"{zoneCode}{sequenceStr}");
            string checKey = (rawNumber % 97).ToString().PadLeft(2, '0');

            // 4. Formatge final: V 00034 006601 50
            return $"V {zoneCode} {sequenceStr} {checKey}";
        }

        public async Task<string> GenerateRequestReferenceAsync(
            WritableDbContext context,
            TimeProvider timeProvider,
            CancellationToken cancellationToken
        )
        {
            // 1. Récupérer l'année courante
            int year = timeProvider.GetUtcNow().Year;

            var lasRequestReference = await context
                .RegistrationRequests.Where(rr => rr.SubmissionDate.Year == year)
                .OrderByDescending(rr => rr.Reference)
                .Select(rr => rr.Reference)
                .FirstOrDefaultAsync(cancellationToken);

            long lastReference = 0;
            if (!string.IsNullOrEmpty(lasRequestReference) && lasRequestReference.StartsWith("DE"))
            {
                var sequenceNumericPart = lasRequestReference[8..];
                _ = long.TryParse(sequenceNumericPart, out lastReference);
            }

            var newReference = lastReference + 1;

            // Format final: DE-2025-0012547
            return $"DE-{year}-{newReference.ToString().PadLeft(7, '0')}";
        }
    }
}
