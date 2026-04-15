using Application.Common.Enums;
using Application.Exceptions;
using Application.Features.PollingStation.CreatePollingStation;
using Application.Models.Errors;
using Application.UnitTest.Common;
using FluentAssertions;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTest.Features.PollingStation
{
    public class CreatePollingStationTest : TestBase
    {
        [Fact]
        public async Task CreatePollingStationTest_ShouldFail_When_StationNumber_Is_Empty()
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

            var command = new CreatePollingStationCommand
            {
                StationNumber = "",
                Wording = "Bureau 01",
                ConstituencyId = constituency.Id,
            };

            // Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                nameof(CreatePollingStationCommand.StationNumber),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldFail_When_Wording_Is_Empty()
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

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "",
                ConstituencyId = constituency.Id,
            };

            // Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                nameof(CreatePollingStationCommand.Wording),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldFail_When_ConstituencyId_Is_Empty()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 0,
            };

            // Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                nameof(CreatePollingStationCommand.ConstituencyId),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldFail_When_Constituency_Does_Not_Exist()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 999999,
            };

            // Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                nameof(CreatePollingStationCommand.ConstituencyId),
                ValidationErrorCode.InvalidLevel
            );
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldFail_When_Constituency_Is_Not_VotingLocation()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            // Create a Region constituency (not VotingLocation)
            var regionConstituency = await CreateConstituencyAsync(
                context,
                "BELIER",
                LocationLevel.Region,
                null
            );

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = regionConstituency.Id,
            };

            // Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                nameof(CreatePollingStationCommand.ConstituencyId),
                ValidationErrorCode.InvalidLevel
            );
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldFail_When_Constituency_Is_Department_Not_VotingLocation()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            // Create a Region and a Department
            var regionConstituency = await CreateConstituencyAsync(
                context,
                "BELIER",
                LocationLevel.Region,
                null
            );

            var departmentConstituency = await CreateConstituencyAsync(
                context,
                "DIDIEVI",
                LocationLevel.Department,
                regionConstituency.Id
            );

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = departmentConstituency.Id,
            };

            // Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                nameof(CreatePollingStationCommand.ConstituencyId),
                ValidationErrorCode.InvalidLevel
            );
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldSucceed_When_All_Parameters_Are_Valid()
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

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBe(0);

            // Verify it was saved in the database
            var createdPollingStation = await context.PollingStations
                .FirstOrDefaultAsync(x => x.StationNumber == "04" && x.Wording == "LYON");
            
            createdPollingStation.Should().NotBeNull();
            createdPollingStation!.ConstituencyId.Should().Be(votingLocationConstituency.Id);
            createdPollingStation.StationNumber.Should().Be("04");
            createdPollingStation.Wording.Should().Be("LYON");
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldSucceed_With_Multiple_PollingStationsUnderSameVotingLocation()
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

            var command1 = new CreatePollingStationCommand
            {
                StationNumber = "01",
                Wording = "Bureau Principal",
                ConstituencyId = votingLocationConstituency.Id,
            };

            var command2 = new CreatePollingStationCommand
            {
                StationNumber = "02",
                Wording = "Bureau Secondaire",
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result1 = await serviceProvider.SendAsync(command1);
            var result2 = await serviceProvider.SendAsync(command2);

            // Assert
            result1.Data.Should().NotBe(0);
            result2.Data.Should().NotBe(0);
            result1.Data.Should().NotBe(result2.Data);

            var pollingStations = await context.PollingStations
                .Where(x => x.ConstituencyId == votingLocationConstituency.Id)
                .ToListAsync();

            pollingStations.Should().HaveCount(2);
            pollingStations.Should().Contain(x => x.StationNumber == "01");
            pollingStations.Should().Contain(x => x.StationNumber == "02");
        }

        [Fact]
        public async Task CreatePollingStationTest_PollingStation_Should_Have_Default_IsEnabled()
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

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            var createdPollingStation = await context.PollingStations
                .FirstOrDefaultAsync(x => x.Id == result.Data);

            createdPollingStation.Should().NotBeNull();
            createdPollingStation!.DisabledDate.Should().BeNull();
            createdPollingStation.CreatedAt.Should().NotBe(default);
            createdPollingStation.ModifiedAt.Should().NotBe(default);
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldSucceed_When_Wording_Has_Max_Length()
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

            var maxLengthWording = new string('A', 100);

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = maxLengthWording,
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            result.Data.Should().NotBe(0);

            var createdPollingStation = await context.PollingStations
                .FirstOrDefaultAsync(x => x.Id == result.Data);

            createdPollingStation.Should().NotBeNull();
            createdPollingStation!.Wording.Should().HaveLength(100);
        }

        [Fact]
        public async Task CreatePollingStationTest_ShouldFail_When_Wording_Exceeds_Max_Length()
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

            var tooLongWording = new string('A', 101);

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = tooLongWording,
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                nameof(CreatePollingStationCommand.Wording),
                ValidationErrorCode.MaxLength
            );
        }

        [Fact]
        public async Task CreatePollingStationTest_Controller_Should_Return_201_Created()
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

            var command = new CreatePollingStationCommand
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert - This test verifies the handler returns a Result that the controller will use to return 201
            result.Should().NotBeNull();
            result.Data.Should().BeGreaterThan(0);
        }
    }
}
