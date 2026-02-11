using Application.Common.Enums;
using Application.Exceptions;
using Application.Features.GeographicArea.CreateGeographicArea;
using Application.Models.Errors;
using Application.UnitTest.Common;
using FluentAssertions;
using FluentValidation.TestHelper;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.EntityFrameworkCore;

namespace Application.UnitTest.Features.GeographicArea
{
    public class CreateGeographicAreaTest : TestBase
    {
        private readonly CreateGeographicAreaCommandValidator _validator;
        private readonly Mock<ReadOnlyDbContext> _mockContext;
        private readonly ReadOnlyDbContext _context;

        public CreateGeographicAreaTest()
        {
            var options = new DbContextOptionsBuilder<ReadOnlyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ReadOnlyDbContext(options);
            _context = context;
            _mockContext = new Mock<ReadOnlyDbContext>(options);
            _validator = new CreateGeographicAreaCommandValidator(_mockContext.Object);
        }

        [Fact]
        public async Task CreateGeographicAreaTest_ShouldFail_When_Name_Is_Empty()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var command = new CreateGeographicAreaCommand
            {
                Name = "",
                Level = LocationLevel.Country,
            };

            //Act
            var result = await _validator.TestValidateAsync(command);

            //Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorCode(ValidationErrorCode.Required.ToString());

            ////Act
            //var result = await FluentActions
            //    .Invoking(() => serviceProvider.SendAsync(command))
            //    .Should()
            //    .ThrowAsync<ValidationException>();

            ////Assert
            //AssertValidationException(
            //    result.Subject,
            //    nameof(CreateGeographicAreaCommand.Name),
            //    ValidationErrorCode.Required
            //);
        }

        [Fact]
        public async Task CreateGeographicAreaTest_ShouldFail_When_Continent_Has_Parent()
        {
            //Arrange
            var command = new CreateGeographicAreaCommand
            {
                Name = "Africa",
                Level = LocationLevel.Continent,
                ParentId = 5,
            };

            //Act
            var result = await _validator.TestValidateAsync(command);

            //Assert
            result
                .ShouldHaveValidationErrorFor(x => x.ParentId)
                .WithErrorCode(ValidationErrorCode.InvalidParent.ToString());
        }

        [Fact]
        public async Task CreateGeographicAreaTest_Should_Have_Error_When_Parent_Level_Is_Wrong()
        {
            //Arrange

            // On veut créer un District (3), donc on s'attend à un parent Country (2)
            var command = new CreateGeographicAreaCommand
            {
                Level = LocationLevel.District,
                ParentId = 50,
            };

            // On mock une base où l'ID 50 est un Continent (1) au lieu d'un Country (2)
            var data = new List<GeographicAreaDao>
            {
                new() { Id = 50, Level = LocationLevel.Continent },
            }.AsQueryable();

            _mockContext.Setup(x => x.GeographicAreas).ReturnsDbSet(data);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.ParentId)
                .WithErrorMessage("Parent must be level Country");
        }

        [Fact]
        public async Task CreateGeographicAreaTest_ShouldFail_When_NameAlreadyExistsUnderSameParent()
        {
            // Arrange : Création d'une sous-préfecture (6) sans parent
            var command = new CreateGeographicAreaCommand
            {
                Name = "Cocody",
                Level = LocationLevel.City,
                ParentId = 10,
            };

            var existingData = new List<GeographicAreaDao>
            {
                new()
                {
                    Name = "Cocody",
                    ParentId = 10,
                    Level = LocationLevel.City,
                },
            };
            _mockContext.Setup(x => x.GeographicAreas).ReturnsDbSet(existingData);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorCode(ValidationErrorCode.AlreadyExists.ToString());
        }

        [Fact]
        public async Task CreateGeographicAreaTest_Validate_ShouldSucceed_When_HierarchyIsConsistent()
        {
            // Arrange : Création d'une ville (7) sous une sous-préfecture (6)
            var command = new CreateGeographicAreaCommand
            {
                Name = "Bingerville",
                Level = LocationLevel.City,
                ParentId = 20,
            };

            var existingData = new List<GeographicAreaDao>
            {
                new() { Id = 20, Level = LocationLevel.SubPrefecture },
            };
            _mockContext.Setup(x => x.GeographicAreas).ReturnsDbSet(existingData);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
