using Application.Common.Enums;
using Application.Features.Users.Common;
using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Configuration;
using Tools.Logging;
using Tools.Serialization;

namespace Application.Features.Users.CreateUser
{
    [WithPermission(nameof(AppPermission.CreateUser))]
    public class CreateUserCommand : UserModel, IRequest<Result<Guid>> 
    {
        [SensitiveData]
        public string? Password { get; set; }
    }

    public class CreateUserCommandValidator: UserCommandValidatorBase<CreateUserCommand>
    {       
        public CreateUserCommandValidator(ReadOnlyDbContext context): base(context)
        {
            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MaximumLength(8)
                .WithMessage(ValidationErrorCode.MinLength.ToString())
                .MaximumLength(50)
                .WithMessage(ValidationErrorCode.MaxLength.ToString());            
        }
        
    }

    public class CreateUserCommandHandler(
        WritableDbContext context,
        UserManager<UserDao> userManager,
        IOptions<AppConfiguration> config,
        //IEmailService emailService,
        TimeProvider timeProvider
    ) : UserCommandHandlerBase(context, userManager,config,timeProvider), 
            IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        
        public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var userDao = new UserDao();
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(
                async () =>
                {
                    await MapToDaoAsync(command, userDao, cancellationToken: cancellationToken);

                    await _userManager.CreateAsync(userDao, command.Password!);

                    activity.AddParameter(userDao, u => u.Id);
                },
                () => Task.FromResult(true)
            );

            return Result<Guid>.From(userDao.Id);
        }
    }
}
