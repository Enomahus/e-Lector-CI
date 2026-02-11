using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;

namespace Application.Features.GeographicArea.Common
{
    public class GeographicAreaCommandValidatorBase<T> : AbstractValidator<T>
        where T : class
    {
        protected readonly ReadOnlyDbContext _context;

        public GeographicAreaCommandValidatorBase(ReadOnlyDbContext context)
        {
            _context = context;
        }
    }
}
