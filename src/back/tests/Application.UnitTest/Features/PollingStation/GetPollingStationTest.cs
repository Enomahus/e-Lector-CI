using Application.Common.Enums;
using Application.Features.PollingStation.CreatePollingStation;
using Application.Features.PollingStation.GetPollingStation;
using Application.UnitTest.Common;
using FluentAssertions;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTest.Features.PollingStation
{
    public class GetPollingStationTest : TestBase
    {
        [Fact]
        public async Task GetPollingStationByIdQuery_ShouldReturnNull_When_PollingStation_Does_Not_Exist()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();

            var query = new GetPollingStationByIdQuery { Id = 999999 };

            // Act
            var result = await serviceProvider.SendAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetPollingStationByIdQuery_ShouldReturnPollingStation_When_It_Exists()
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

            // Act
            var query = new GetPollingStationByIdQuery { Id = pollingStationId };
            var result = await serviceProvider.SendAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data!.StationNumber.Should().Be("04");
            result.Data.Wording.Should().Be("LYON");
            result.Data.ConstituencyId.Should().Be(constituency.Id);
            result.Data.IsActive.Should().BeTrue();
        }
    }
}
