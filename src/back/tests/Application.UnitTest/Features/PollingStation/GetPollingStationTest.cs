using Application.Common.Enums;
using Application.Features.PollingStation.CreatePollingStation;
using Application.Features.PollingStation.GetPollingStation;
using Application.Features.PollingStation.GetPollingStations;
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

        //[Fact]
        //public async Task GetPollingStationsByConstituencyIdQuery_ShouldReturnEmptyList_When_No_PollingStations_Exist()
        //{
        //    // Arrange
        //    var serviceProvider = CreateServiceCollection().BuildServiceProvider();
        //    var context = serviceProvider.GetRequiredService<WritableDbContext>();

        //    var constituency = await CreateConstituencyAsync(
        //        context,
        //        "EPP ALLANIKRO",
        //        LocationLevel.VotingLocation,
        //        null
        //    );

        //    var query = new GetPollingStationsQuery();

        //    // Act
        //    var result = await serviceProvider.SendAsync(query);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Data.Should().NotBeNull();
        //    result.Data.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task GetPollingStationsByConstituencyIdQuery_ShouldReturnAllPollingStations_When_They_Exist()
        //{
        //    // Arrange
        //    var serviceProvider = CreateServiceCollection().BuildServiceProvider();
        //    var context = serviceProvider.GetRequiredService<WritableDbContext>();

        //    var constituency = await CreateConstituencyAsync(
        //        context,
        //        "EPP ALLANIKRO",
        //        LocationLevel.VotingLocation,
        //        null
        //    );

        //    var command1 = new CreatePollingStationCommand
        //    {
        //        StationNumber = "01",
        //        Wording = "Bureau Principal",
        //        ConstituencyId = constituency.Id,
        //    };

        //    var command2 = new CreatePollingStationCommand
        //    {
        //        StationNumber = "02",
        //        Wording = "Bureau Secondaire",
        //        ConstituencyId = constituency.Id,
        //    };

        //    await serviceProvider.SendAsync(command1);
        //    await serviceProvider.SendAsync(command2);

        //    var query = new GetPollingStationsQuery();

        //    // Act
        //    var result = await serviceProvider.SendAsync(query);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Data.Should().NotBeNull();
        //    result.Data.Should().HaveCount(2);
        //    result.Data.Should().Contain(x => x.StationNumber == "01");
        //    result.Data.Should().Contain(x => x.StationNumber == "02");
        //}

        //[Fact]
        //public async Task GetPollingStationsByConstituencyIdQuery_ShouldNotReturnPollingStations_From_Other_Constituencies()
        //{
        //    // Arrange
        //    var serviceProvider = CreateServiceCollection().BuildServiceProvider();
        //    var context = serviceProvider.GetRequiredService<WritableDbContext>();

        //    var constituency1 = await CreateConstituencyAsync(
        //        context,
        //        "EPP ALLANIKRO 1",
        //        LocationLevel.VotingLocation,
        //        null
        //    );

        //    var constituency2 = await CreateConstituencyAsync(
        //        context,
        //        "EPP ALLANIKRO 2",
        //        LocationLevel.VotingLocation,
        //        null
        //    );

        //    var command1 = new CreatePollingStationCommand
        //    {
        //        StationNumber = "01",
        //        Wording = "Bureau 1",
        //        ConstituencyId = constituency1.Id,
        //    };

        //    var command2 = new CreatePollingStationCommand
        //    {
        //        StationNumber = "02",
        //        Wording = "Bureau 2",
        //        ConstituencyId = constituency2.Id,
        //    };

        //    await serviceProvider.SendAsync(command1);
        //    await serviceProvider.SendAsync(command2);

        //    var query = new GetPollingStationsQuery();

        //    // Act
        //    var result = await serviceProvider.SendAsync(query);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Data.Should().NotBeNull();
        //    result.Data.Should().HaveCount(1);
        //    result.Data.Should().Contain(x => x.StationNumber == "01");
        //    result.Data.Should().NotContain(x => x.StationNumber == "02");
        //}
    }
}
