using Application.Features.Common.PollingStation;
using FluentAssertions;
using Infrastructure.Persistence.Entities;

namespace Application.UnitTest.Features.PollingStation
{
    public class PollingStationModelTest
    {
        [Fact]
        public void PollingStationModel_FromDao_Should_Map_All_Properties()
        {
            // Arrange
            var dateNow = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
            var dao = new PollingStationDao
            {
                Id = 1,
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 100,
                DisabledDate = null,
                CreatedAt = dateNow,
                ModifiedAt = dateNow,
            };

            // Act
            var model = PollingStationModel.FromDao(dao, dateNow);

            // Assert
            model.Should().NotBeNull();
            model.StationNumber.Should().Be("04");
            model.Wording.Should().Be("LYON");
            model.ConstituencyId.Should().Be(100);
            model.IsActive.Should().BeTrue();
        }

        [Fact]
        public void PollingStationModel_FromDao_Should_Mark_As_Inactive_When_DisabledDate_Is_Before_Now()
        {
            // Arrange
            var dateNow = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
            var disabledDate = new DateTimeOffset(2026, 1, 1, 9, 0, 0, TimeSpan.Zero); // Before dateNow

            var dao = new PollingStationDao
            {
                Id = 1,
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 100,
                DisabledDate = disabledDate,
                CreatedAt = dateNow,
                ModifiedAt = dateNow,
            };

            // Act
            var model = PollingStationModel.FromDao(dao, dateNow);

            // Assert
            model.IsActive.Should().BeFalse();
        }

        [Fact]
        public void PollingStationModel_FromDao_Should_Mark_As_Active_When_DisabledDate_Is_After_Now()
        {
            // Arrange
            var dateNow = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
            var disabledDate = new DateTimeOffset(2026, 1, 1, 11, 0, 0, TimeSpan.Zero); // After dateNow

            var dao = new PollingStationDao
            {
                Id = 1,
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 100,
                DisabledDate = disabledDate,
                CreatedAt = dateNow,
                ModifiedAt = dateNow,
            };

            // Act
            var model = PollingStationModel.FromDao(dao, dateNow);

            // Assert
            model.IsActive.Should().BeTrue();
        }

        [Fact]
        public void PollingStationModel_ToDao_Should_Map_All_Properties()
        {
            // Arrange
            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 100,
                IsActive = true,
            };

            // Act
            var dao = model.ToDao();

            // Assert
            dao.Should().NotBeNull();
            dao.StationNumber.Should().Be("04");
            dao.Wording.Should().Be("LYON");
            dao.ConstituencyId.Should().Be(100);
            dao.DisabledDate.Should().BeNull();
        }

        [Fact]
        public void PollingStationModel_ToDao_Should_Create_New_Instance()
        {
            // Arrange
            var model = new PollingStationModel
            {
                StationNumber = "04",
                Wording = "LYON",
                ConstituencyId = 100,
            };

            // Act
            var dao1 = model.ToDao();
            var dao2 = model.ToDao();

            // Assert
            dao1.Should().NotBeSameAs(dao2);
            dao1.StationNumber.Should().Be(dao2.StationNumber);
            dao1.Wording.Should().Be(dao2.Wording);
            dao1.ConstituencyId.Should().Be(dao2.ConstituencyId);
        }

        [Fact]
        public void PollingStationModel_FromDao_And_Back_Should_Preserve_Data()
        {
            // Arrange
            var dateNow = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
            var originalDao = new PollingStationDao
            {
                Id = 42,
                StationNumber = "25",
                Wording = "ABIDJAN",
                ConstituencyId = 555,
                DisabledDate = null,
                CreatedAt = dateNow,
                ModifiedAt = dateNow,
            };

            // Act
            var model = PollingStationModel.FromDao(originalDao, dateNow);
            var newDao = model.ToDao();

            // Assert
            newDao.StationNumber.Should().Be(originalDao.StationNumber);
            newDao.Wording.Should().Be(originalDao.Wording);
            newDao.ConstituencyId.Should().Be(originalDao.ConstituencyId);
        }

        [Fact]
        public void PollingStationModel_Properties_Should_Be_Nullable()
        {
            // Arrange & Act
            var model = new PollingStationModel();

            // Assert
            model.StationNumber.Should().BeNull();
            model.Wording.Should().BeNull();
            model.ConstituencyId.Should().Be(0);
            model.IsActive.Should().BeFalse();
        }
    }
}
