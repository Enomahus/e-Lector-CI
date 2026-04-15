using Application.Common.Enums;
using Application.Exceptions;
using Application.Features.PollingStation.CreatePollingStation;
using Application.Features.PollingStation.UpdatePollingStation;
using Application.Models.Errors;
using Application.UnitTest.Common;
using FluentAssertions;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTest.Features.PollingStation
{
    public class UpdatePollingStationTest : TestBase
    {
        [Fact]
        public async Task UpdatePollingStationCommand_ShouldFail_When_StationNumber_Is_Empty()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            // Act & Assert
            await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            AssertValidationException(
                () => throw new ValidationException(
                    new List<ValidationFailure>
                    {
                        new ValidationFailure(
                            nameof(UpdatePollingStationCommand.StationNumber),
                            ValidationErrorCode.Required.ToString()
                        ),
                    }
                ),
                nameof(UpdatePollingStationCommand.StationNumber),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldFail_When_Wording_Is_Empty()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "04",
                Wording = "",
                ConstituencyId = constituency.Id,
            };

            // Act & Assert
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            AssertValidationException(
                result.Subject,
                nameof(UpdatePollingStationCommand.Wording),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldFail_When_ConstituencyId_Is_Empty()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 0,
            };

            // Act & Assert
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            AssertValidationException(
                result.Subject,
                nameof(UpdatePollingStationCommand.ConstituencyId),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldFail_When_Constituency_Does_Not_Exist()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 999999,
            };

            // Act & Assert
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            AssertValidationException(
                result.Subject,
                nameof(UpdatePollingStationCommand.ConstituencyId),
                ValidationErrorCode.InvalidLevel
            );
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldFail_When_Constituency_Is_Not_VotingLocation()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var votingLocationConstituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var regionConstituency = await CreateConstituencyAsync(
                context,
                "BELIER",
                LocationLevel.Region,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = votingLocationConstituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = regionConstituency.Id,
            };

            // Act & Assert
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            AssertValidationException(
                result.Subject,
                nameof(UpdatePollingStationCommand.ConstituencyId),
                ValidationErrorCode.InvalidLevel
            );
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldSucceed_When_All_Parameters_Are_Valid()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "05",
                Wording = "MARSEILLE",
                ConstituencyId = constituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().Be(pollingStationId);

            // Verify it was updated in the database
            var updatedPollingStation = await context.PollingStations
                .FirstOrDefaultAsync(x => x.Id == pollingStationId);

            updatedPollingStation.Should().NotBeNull();
            updatedPollingStation!.StationNumber.Should().Be("05");
            updatedPollingStation.Wording.Should().Be("MARSEILLE");
            updatedPollingStation.ConstituencyId.Should().Be(constituency.Id);
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldReturnZero_When_PollingStation_Does_Not_Exist()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var command = new UpdatePollingStationCommand
            {
                Id = 999999,
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().Be(0);
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldSucceed_When_Only_StationNumber_Changes()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "10",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().Be(pollingStationId);

            var updatedPollingStation = await context.PollingStations
                .FirstOrDefaultAsync(x => x.Id == pollingStationId);

            updatedPollingStation.Should().NotBeNull();
            updatedPollingStation!.StationNumber.Should().Be("10");
        }

        [Fact]
        public async Task UpdatePollingStationCommand_ShouldSucceed_When_Only_Wording_Changes()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituency = await CreateConstituencyAsync(
                context,
                "EPP ALLANIKRO",
                LocationLevel.VotingLocation,
                null
            );

            var createCommand = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = constituency.Id,
            };

            var createResult = await serviceProvider.SendAsync(createCommand);
            var pollingStationId = createResult.Data;

            var command = new UpdatePollingStationCommand
            {
                Id = pollingStationId,
                StationNumber = "04",
                Wording = "PARIS",
                ConstituencyId = constituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().Be(pollingStationId);

            var updatedPollingStation = await context.PollingStations
                .FirstOrDefaultAsync(x => x.Id == pollingStationId);

            updatedPollingStation.Should().NotBeNull();
            updatedPollingStation!.Wording.Should().Be("PARIS");
        }
    }
}
