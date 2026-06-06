using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Enums;
using Application.Exceptions;
using Application.Exceptions.Auth;
using Application.Features.Citizen.CreateBasicCitizen;
using Application.Models.Errors;
using Application.UnitTest.Common;
using FluentAssertions;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTest.Features.Citizen
{
    public class CreateBasicCitizenTest : TestBase
    {
        [Fact]
        public async Task CreateBasicCitizenTest_ShouldFail_WhenPermissionMissing()
        {
            //Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();

            var request = new CreateBasicCitizenCommand();

            // Act & Assert
            var result = await FluentActions
                .Invoking(() => serviceProvider.SendAsync(request))
                .Should()
                .ThrowAsync<UserAccessException>();
        }

        [Fact]
        public async Task CreateBasicCitizenTest_ShouldReturnValidationException_WhenFieldsAreEmpty()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            await SetupCurrentUserAsync(serviceProvider, permissions: [AppPermission.CreateBasicCitizen]);

            var command = new CreateBasicCitizenCommand();

            // Act
            //var result = await FluentActions
            //    .Invoking(() => serviceProvider.SendAsync(command))
            //    .Should()
            //    .ThrowAsync<ValidationException>();
            var action = () => serviceProvider.SendAsync(command);

            // Assert

            var exceptionAssertion = await action.Should().ThrowAsync<ValidationException>();
            var exception = exceptionAssertion.And;

            AssertValidationException(
                exception,
                //result.Subject,
                new KeyValuePair<string, ValidationErrorCode>("firstName", ValidationErrorCode.Required),
                new KeyValuePair<string, ValidationErrorCode>("lastName", ValidationErrorCode.Required),
                new KeyValuePair<string, ValidationErrorCode>("birthDate", ValidationErrorCode.Required),
                new KeyValuePair<string, ValidationErrorCode>("birthPlace", ValidationErrorCode.Required),
                new KeyValuePair<string, ValidationErrorCode>("nationality", ValidationErrorCode.Required)
            );
        }

        [Fact]
        public async Task CreateBasicCitizenTest_ShouldReturnId_WhenCreationIsSuccessful()
        {
            // Arrange
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();
            await SetupCurrentUserAsync(serviceProvider, permissions: [AppPermission.CreateBasicCitizen]);
            var context = serviceProvider.GetRequiredService<WritableDbContext>();

            var command = new CreateBasicCitizenCommand()
            {
                Gender = Gender.M,
                FirstName = "Dynozoph",
                LastName = "YAO",
                BirthDate = new DateTimeOffset(1975, 1, 1, 0, 0, 0, TimeSpan.Zero),
                BirthPlace = "Daloa",
                Nationality = "Ivoirienne",
            };

            // Act
            var result = await serviceProvider.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBe(Guid.Empty);

            var citizen = await context.Citizens.AsNoTracking().FirstOrDefaultAsync(c => c.Id == result.Data);

            citizen.Should().NotBeNull();
            citizen!.Gender.Should().Be(Gender.M);
            citizen!.FirstName.Should().Be(command.FirstName);
            citizen.LastName.Should().Be(command.LastName);
            citizen.BirthPlace.Should().Be(command.BirthPlace);
            citizen.BirthDate.Should().Be(command.BirthDate);
            citizen.Nationality.Should().Be(command.Nationality);
        }
    }
}
