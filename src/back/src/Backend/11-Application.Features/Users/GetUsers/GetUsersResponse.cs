using Application.Common.Enums;

namespace Application.Features.Users.GetUsers
{
    public class GetUsersResponse
    {
        public required Guid UserId { get; init; }
        public required string LastName { get; init; }
        public required string FirstName { get; init; }
        public PersonTitle Civility { get; init; }
        public required string? Email { get; init; }
        public required string? Phone { get; init; }
        public required string? EmployeeNumber { get; init; }
        public required UserType UserType { get; init; }
        public required bool IsActive { get; init; }
        public required string? Constituency { get; init; }
        public required bool CanBeDeleted { get; init; }
        public required bool CanBeToggled { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public IEnumerable<string>? Roles { get; init; }
        public AuthProvider? AuthProvider { get; init; }

    }
}
