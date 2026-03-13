using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;

namespace Application.Features.Constituency.Common
{
    public class ConstituencyCommandValidatorBase<T> : AbstractValidator<T>
        where T : class
    {
        protected readonly ReadOnlyDbContext _context;

        public ConstituencyCommandValidatorBase(ReadOnlyDbContext context)
        {
            _context = context;
        }
    }
}
