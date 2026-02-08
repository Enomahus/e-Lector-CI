using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.SQLServer.Contexts;

public class WritableDbContext : ApplicationDbContext
{
    public WritableDbContext() { }

    public WritableDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
