using Application.Common.Enums;
using Application.Features.Common.PollingStation;
using Application.Models.Errors;
using Application.UnitTest.Common;
using FluentAssertions;
using FluentValidation.TestHelper;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq.EntityFrameworkCore;

namespace Application.UnitTest.Features.PollingStation
{
    public class PollingStationValidatorTest : TestBase
    {
        [Fact]
        public async Task PollingStationValidator_StationNumber_Should_Be_Required()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(context);

            var model = new PollingStationModel
            {
                StationNumber = "",
                Wording = "LYON",
                ConstituencyId = 1,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StationNumber)
                .WithErrorCode(ValidationErrorCode.Required.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_Wording_Should_Be_Required()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(context);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "",
                ConstituencyId = 1,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Wording)
                .WithErrorCode(ValidationErrorCode.Required.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_Wording_Should_Not_Exceed_MaxLength()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(context);

            var tooLongWording = new string('A', 101);
            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = tooLongWording,
                ConstituencyId = 1,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Wording)
                .WithErrorCode(ValidationErrorCode.MaxLength.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_ConstituencyId_Should_Be_Required()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(context);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 0,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConstituencyId)
                .WithErrorCode(ValidationErrorCode.Required.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_ConstituencyId_Should_Reference_VotingLocation()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var regionConstituency = await CreateConstituencyAsync(
                context,
                "BELIER",
                LocationLevel.Region,
                null
            );

            var roContext = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(roContext);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = regionConstituency.Id,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConstituencyId)
                .WithErrorCode(ValidationErrorCode.InvalidLevel.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_Should_Pass_For_Valid_VotingLocation()
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

            var roContext = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(roContext);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task PollingStationValidator_Should_Pass_For_MaxLength_Wording()
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

            var roContext = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(roContext);

            var maxLengthWording = new string('A', 100);
            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = maxLengthWording,
                ConstituencyId = votingLocationConstituency.Id,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task PollingStationValidator_Should_Fail_For_NonExistentConstituency()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var roContext = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(roContext);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 999999,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConstituencyId)
                .WithErrorCode(ValidationErrorCode.InvalidLevel.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_Should_Fail_For_Department_Level()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

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

            var roContext = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(roContext);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = departmentConstituency.Id,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConstituencyId)
                .WithErrorCode(ValidationErrorCode.InvalidLevel.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_Should_Fail_For_SubPrefecture_Level()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

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

            var subPrefectureConstituency = await CreateConstituencyAsync(
                context,
                "BOLI",
                LocationLevel.SubPrefecture,
                departmentConstituency.Id
            );

            var roContext = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(roContext);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = subPrefectureConstituency.Id,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConstituencyId)
                .WithErrorCode(ValidationErrorCode.InvalidLevel.ToString());
        }

        [Fact]
        public async Task PollingStationValidator_Should_Fail_For_Municipality_Level()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

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

            var subPrefectureConstituency = await CreateConstituencyAsync(
                context,
                "BOLI",
                LocationLevel.SubPrefecture,
                departmentConstituency.Id
            );

            var municipalityConstituency = await CreateConstituencyAsync(
                context,
                "BOLI MUNICIPALITY",
                LocationLevel.Municipality,
                subPrefectureConstituency.Id
            );

            var roContext = serviceProvider.GetRequiredService<ReadOnlyDbContext>();
            var validator = new PollingStationValidatorBase<PollingStationModel>(roContext);

            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = municipalityConstituency.Id,
            };

            // Act
            var result = await validator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConstituencyId)
                .WithErrorCode(ValidationErrorCode.InvalidLevel.ToString());
        }
    }
}
