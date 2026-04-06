using Application.Common.Enums;
using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.Users.DeleteUser;

[WithPermission(nameof(AppPermission.DeleteUser))]
public class DeleteUserCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly ReadOnlyDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserCommandValidator(ReadOnlyDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
        RuleFor(u => u.Id)
            .NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString())
            .DependentRules(() =>
            {
                RuleFor(u => u.Id)
                    .MustAsync(
                        (userId, cancellationToken) =>
                            _context.Users.AnyAsync(u => u.Id == userId, cancellationToken)
                    )
                    .WithMessage(ValidationErrorCode.UserMustExist.ToString())
                    .DependentRules(() =>
                    {
                        RuleFor(u => u.Id as Guid?)
                            .NotEqual(_currentUserService.UserId)
                            .WithMessage(ValidationErrorCode.UserCannotBeCurrentUser.ToString())
                            .MustAsync(
                                (id, token) =>
                                {
                                    return CheckUserLinksAsync(id, token);
                                }
                            )
                            .WithMessage(ValidationErrorCode.UserLinked.ToString());
                    });
            });
    }

    private Task<bool> CheckUserLinksAsync(Guid? userId, CancellationToken cancellationToken)
    {
        return _context.Users.AnyAsync(
            u => u.Id == userId && u.CreatedRegistrationRequests.Count == 0,
            cancellationToken
        );
    }
}

public class DeleteUserCommandHandler(WritableDbContext context) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        using var activity = ActivitySourceLog.CQRS.Start().AddParameter(command, r => r.Id);

        var user = await context.Users.FirstAsync(s => s.Id == command.Id, cancellationToken);

        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Default();
    }
}
