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
using System;

namespace Application.UnitTest.Features.GeographicArea
{
    public class CreateGeographicAreaTest : TestBase
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
                nameof(CreateGeographicAreaCommand.Name),
                ValidationErrorCode.Required
            );
        }

        [Fact]
        public async Task CreateGeographicAreaTest_ShouldFail_When_Continent_Has_Parent()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var command = new CreateGeographicAreaCommand
            {
                Name = "Africa",
                Level = LocationLevel.Continent,
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
                    nameof(CreateGeographicAreaCommand.ParentId),
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
        public async Task CreateGeographicAreaTest_Should_Have_Error_When_Parent_Level_Is_Wrong()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var geographicArea = await CreateGeographicAreaAsync(context,"Afrique",LocationLevel.Continent,1);
            // On veut créer un District (3), donc on s'attend à un parent Country (2)
            var command = new CreateGeographicAreaCommand
            {
                Name = "Korhogo",
                Level = LocationLevel.District,
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
                    nameof(CreateGeographicAreaCommand.ParentId),
                    ValidationErrorCode.InvalidLevel
                )
            );

            // On mock une base où l'ID 50 est un Continent (1) au lieu d'un Country (2)
            //var data = new List<GeographicAreaDao>
            //{
            //    new() { Id = 50, Level = LocationLevel.Continent },
            //}.AsQueryable();

            //_mockContext.Setup(x => x.GeographicAreas).ReturnsDbSet(data);

            //// Act
            //var result = await _validator.TestValidateAsync(command);

            //// Assert
            //result
            //    .ShouldHaveValidationErrorFor(x => x.ParentId)
            //    .WithErrorMessage("Parent must be level Country");
        }

        [Fact]
        public async Task CreateGeographicAreaTest_ShouldFail_When_NameAlreadyExistsUnderSameParent()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var geographicAreaDistrict = await context.GeographicAreas.FirstOrDefaultAsync(x => x.Name == "ABIDJAN" && x.Level == LocationLevel.District);

            var command = new CreateGeographicAreaCommand
            {
                Name = "Cocody",
                Level = LocationLevel.City,
                ParentId = geographicAreaDistrict?.Id,
            };

            var existingData = new List<GeographicAreaDao>
            {
                new()
                {
                    Name = "Cocody",
                    ParentId = geographicAreaDistrict?.Id,
                    Level = LocationLevel.City,
                },
                new()
                {
                    Name = "Plateau",
                    ParentId = geographicAreaDistrict?.Id,
                    Level = LocationLevel.City,
                },
                new()
                {
                    Name = "Adjamé",
                    ParentId = geographicAreaDistrict?.Id,
                    Level = LocationLevel.City,
                },
            };

            await context.GeographicAreas.AddRangeAsync(existingData);
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
                    nameof(CreateGeographicAreaCommand.Name),
                    ValidationErrorCode.AlreadyExists
                )
            );


            //_mockContext.Setup(x => x.GeographicAreas).ReturnsDbSet(existingData);

            //// Act
            //var result = await _validator.TestValidateAsync(command);

            //// Assert
            //result
            //    .ShouldHaveValidationErrorFor(x => x.Name)
            //    .WithErrorCode(ValidationErrorCode.AlreadyExists.ToString());
        }

        [Fact]
        public async Task CreateGeographicAreaTest_Validate_ShouldSucceed_When_HierarchyIsConsistent()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var geographicAreaDistrict = await context.GeographicAreas
                .FirstOrDefaultAsync(x => x.Name == "BINGERVILLE" && x.Level == LocationLevel.SubPrefecture);

            // Arrange : Création d'une ville (7) sous une sous-préfecture (6)
            var command = new CreateGeographicAreaCommand
            {
                Name = "Bingerville",
                Level = LocationLevel.City,
                ParentId = 12,
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
            //_mockContext.Setup(x => x.GeographicAreas).ReturnsDbSet(existingData);

            //// Act
            //var result = await _validator.TestValidateAsync(command);

            //// Assert
            //result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
