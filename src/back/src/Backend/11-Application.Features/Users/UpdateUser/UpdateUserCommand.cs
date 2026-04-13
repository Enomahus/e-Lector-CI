using Application.Common.Enums;
using Application.Exceptions;
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

namespace Application.Features.Users.UpdateUser;

[WithPermission(nameof(AppPermission.UpdateUser))]
public class UpdateUserCommand : UserModel, IRequest<Result<Guid>>
{
    public Guid UserId { get; set; }
}

public class UpdateUserCommandValidator : UserCommandValidatorBase<UpdateUserCommand>
{
    public UpdateUserCommandValidator(ReadOnlyDbContext context)
        : base(context)
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
    }

    protected override Task<bool> BeUniqueEmailAsync(
        UpdateUserCommand command,
        string email,
        CancellationToken cancellationToken
    )
    {
        return _context.Users.AllAsync(
            u => command.UserId == u.Id || u.UserName != email && u.Email != email,
            cancellationToken
        );
    }
}

public class UpdateUserCommandHandler(
    WritableDbContext context,
    UserManager<UserDao> userManager,
    IOptions<AppConfiguration> config,
    //IEmailService emailService,
    TimeProvider timeProvider
)
    : UserCommandHandlerBase(context, userManager, config, timeProvider),
        IRequestHandler<UpdateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        using var activity = ActivitySourceLog.CQRS.Start();

        var userDao =
            await _context
                .Users.Include(u => u.UserRoles)
                .Include(u => u.UserConstituencies)
                .Where(u => u.Id == command.UserId)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(UserDao), command.UserId);

        var strategy = _context.Database.CreateExecutionStrategy();
        var sendAccountConfirmation = userDao.Email != command.Email;
        await strategy.ExecuteInTransactionAsync(
            async () =>
            {
                await MapToDaoAsync(command, userDao, cancellationToken: cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            },
            () => Task.FromResult(true)
        );
        if (sendAccountConfirmation)
        {
            await SendCreatePasswordEmailAsync(userDao);
        }

        return Result<Guid>.From(userDao.Id);
    }
}
