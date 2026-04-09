using System;
using Application.Common.Enums;
using Application.Exceptions;
using Application.Features.Constituency.CreateConstituency;
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

namespace Application.UnitTest.Features.Constituency
{
    public class CreateConstituencyTest : TestBase
    {
        //private readonly CreateGeographicAreaCommandValidator _validator;
        //private readonly Mock<ReadOnlyDbContext> _mockContext;
        //private readonly ReadOnlyDbContext _context;

        //public CreateGeographicAreaTest()
        //{
        //    var options = new DbContextOptionsBuilder<ReadOnlyDbContext>()
        //        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //        .Options;
        //    var context = new ReadOnlyDbContext(options);
        //    _context = context;
        //    _mockContext = new Mock<ReadOnlyDbContext>(options);
        //    _validator = new CreateGeographicAreaCommandValidator(_mockContext.Object);
        //}

        [Fact]
        public async Task CreateConstituencyTest_ShouldFail_When_Code_Is_Empty()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var command = new CreateConstituencyCommand
            {
                Wording = "Name",
                Code = "",
                Level = LocationLevel.Region,
            };

            ////Act
            //var result = await _validator.TestValidateAsync(command);

            ////Assert
            //result
            //    .ShouldHaveValidationErrorFor(x => x.Name)
            //    .WithErrorCode(ValidationErrorCode.Required.ToString());

            //Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            //Assert
            AssertValidationException(
                result.Subject,
                nameof(CreateConstituencyCommand.Code),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task CreateConstituencyTest_ShouldFail_When_Name_Is_Empty()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var command = new CreateConstituencyCommand
            {
                Wording = "",
                Code = "BLR",
                Level = LocationLevel.Region,
            };

            ////Act
            //var result = await _validator.TestValidateAsync(command);

            ////Assert
            //result
            //    .ShouldHaveValidationErrorFor(x => x.Name)
            //    .WithErrorCode(ValidationErrorCode.Required.ToString());

            //Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            //Assert
            AssertValidationException(
                result.Subject,
                nameof(CreateConstituencyCommand.Wording),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task CreateConstituencyTest_ShouldFail_When_Region_Has_Parent()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var command = new CreateConstituencyCommand
            {
                Wording = "BELIER",
                Code = "BLR",
                Level = LocationLevel.Region,
                ParentId = 5,
            };

            //Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            //Assert
            AssertValidationException(
                result.Subject,
                new KeyValuePair<string, ValidationErrorCode>(
                    nameof(CreateConstituencyCommand.ParentId),
                    ValidationErrorCode.InvalidParent
                )
            );

            ////Act
            //var result = await _validator.TestValidateAsync(command);

            ////Assert
            //result
            //    .ShouldHaveValidationErrorFor(x => x.ParentId)
            //    .WithErrorCode(ValidationErrorCode.InvalidParent.ToString());
        }

        [Fact]
        public async Task CreateConstituencyTest_Should_Have_Error_When_Parent_Level_Is_Wrong()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var geographicArea = await CreateConstituencyAsync(
                context,
                "Afrique",
                LocationLevel.Region,
                1
            );
            // On veut créer un District (3), donc on s'attend à un parent Country (2)
            var command = new CreateConstituencyCommand
            {
                Wording = "Korhogo",
                Level = LocationLevel.Municipality,
                ParentId = geographicArea.Id,
            };
            //Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            //Assert
            AssertValidationException(
                result.Subject,
                new KeyValuePair<string, ValidationErrorCode>(
                    nameof(CreateConstituencyCommand.ParentId),
                    ValidationErrorCode.InvalidLevel
                )
            );

            // On mock une base où l'ID 50 est un Continent (1) au lieu d'un Country (2)
            //var data = new List<GeographicAreaDao>
            //{
            //    new() { Id = 50, Level = LocationLevel.Continent },
            //}.AsQueryable();

            //_mockContext.Setup(x => x.Constituencies).ReturnsDbSet(data);

            //// Act
            //var result = await _validator.TestValidateAsync(command);

            //// Assert
            //result
            //    .ShouldHaveValidationErrorFor(x => x.ParentId)
            //    .WithErrorMessage("Parent must be level Country");
        }

        [Fact]
        public async Task CreateConstituencyTest_ShouldFail_When_NameAlreadyExistsUnderSameParent()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituencyRegion = await context.Constituencies.FirstOrDefaultAsync(x =>
                x.Wording == "RAVIART" && x.Level == LocationLevel.SubPrefecture
            );

            var command = new CreateConstituencyCommand
            {
                Wording = "Cocody",
                Level = LocationLevel.Municipality,
                ParentId = constituencyRegion?.Id,
            };

            var existingData = new List<ConstituencyDao>
            {
                new()
                {
                    Wording = "Cocody",
                    ParentId = constituencyRegion?.Id,
                    Level = LocationLevel.Municipality,
                },
                new()
                {
                    Wording = "Plateau",
                    ParentId = constituencyRegion?.Id,
                    Level = LocationLevel.Municipality,
                },
                new()
                {
                    Wording = "Adjamé",
                    ParentId = constituencyRegion?.Id,
                    Level = LocationLevel.Municipality,
                },
            };

            await context.Constituencies.AddRangeAsync(existingData);
            await context.SaveChangesAsync();

            //Act
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(command))
                .Should()
                .ThrowAsync<ValidationException>();

            // Assert
            AssertValidationException(
                result.Subject,
                new KeyValuePair<string, ValidationErrorCode>(
                    nameof(CreateConstituencyCommand.Wording),
                    ValidationErrorCode.AlreadyExists
                )
            );

            //_mockContext.Setup(x => x.Constituencies).ReturnsDbSet(existingData);

            //// Act
            //var result = await _validator.TestValidateAsync(command);

            //// Assert
            //result
            //    .ShouldHaveValidationErrorFor(x => x.Name)
            //    .WithErrorCode(ValidationErrorCode.AlreadyExists.ToString());
        }

        [Fact]
        public async Task CreateConstituencyTest_Validate_ShouldSucceed_When_HierarchyIsConsistent()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var constituencyRegion = await context.Constituencies.FirstOrDefaultAsync(x =>
                x.Wording == "DISTRICT AUTONOME D'ABIDJAN" && x.Level == LocationLevel.Region
            );

            // Arrange : Création d'une ville (7) sous une sous-préfecture (6)
            var command = new CreateConstituencyCommand
            {
                Wording = "ABIDJAN",
                Code = "ABJ",
                Level = LocationLevel.Department,
                ParentId = constituencyRegion?.Id,
            };

            //Act
            var result = await serviceProvider.SendAsync(command);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBe(0);

            //var existingData = new List<GeographicAreaDao>
            //{
            //    new() { Id = 20, Level = LocationLevel.SubPrefecture },
            //};
            //_mockContext.Setup(x => x.Constituencies).ReturnsDbSet(existingData);

            //// Act
            //var result = await _validator.TestValidateAsync(command);

            //// Assert
            //result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
