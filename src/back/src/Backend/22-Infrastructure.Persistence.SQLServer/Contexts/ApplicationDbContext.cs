using Application.Common.Enums;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.SQLServer.Contexts;

public class ApplicationDbContext
    : IdentityDbContext<
        UserDao,
        RoleDao,
        Guid,
        IdentityUserClaim<Guid>,
        UserRoleDao,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >
{
    public DbSet<RefreshTokenDao> RefreshTokens { get; set; }
    public DbSet<ConstituencyDao> Constituencies { get; set; }
    public DbSet<PollingStationDao> PollingStations { get; set; }
    public DbSet<RegistrationRequestDao> RegistrationRequests { get; set; }
    public DbSet<RegistrationRequestDocumentDao> RegistrationRequestDocuments { get; set; }
    public DbSet<ElectorDao> Electors { get; set; }
    public DbSet<DocumentDao> Documents { get; set; }
    public DbSet<FiliationDao> Filiations { get; set; }
    public DbSet<CitizenDao> Citizens { get; set; }

    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DocumentType>()
            .HaveConversion<EnumToStringConverter<DocumentType>>();

        configurationBuilder
            .Properties<PersonTitle>()
            .HaveConversion<EnumToStringConverter<PersonTitle>>();

        configurationBuilder
            .Properties<LocationLevel>()
            .HaveConversion<EnumToStringConverter<LocationLevel>>();

        configurationBuilder.Properties<Gender>().HaveConversion<EnumToStringConverter<Gender>>();

        configurationBuilder
            .Properties<MaritalStatus>()
            .HaveConversion<EnumToStringConverter<MaritalStatus>>();

        configurationBuilder
            .Properties<FiliationType>()
            .HaveConversion<EnumToStringConverter<FiliationType>>();

        configurationBuilder
            .Properties<RegistrationStatus>()
            .HaveConversion<EnumToStringConverter<RegistrationStatus>>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserDao>().Property(e => e.UserName).HasMaxLength(100);
        builder.Entity<UserDao>().Property(e => e.FirstName).HasMaxLength(100);
        builder.Entity<UserDao>().Property(e => e.LastName).HasMaxLength(100);
        builder.Entity<UserDao>().Property(e => e.Email).HasMaxLength(100);

        builder
            .Entity<UserDao>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(r => r.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<UserDao>()
            .HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<UserRoleDao>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<UserRoleDao>()
            .HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => e.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<RegistrationRequestDao>()
            .HasOne(l => l.Author)
            .WithMany(a => a.CreatedRegistrationRequests)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<RegistrationRequestDao>()
            .HasOne(l => l.LastUpdater)
            .WithMany(a => a.UpdatedRegistrationRequests)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<CitizenDao>()
            .HasOne(e => e.Elector)
            .WithOne(c => c.Citizen)
            .HasForeignKey<ElectorDao>(e => e.CitizenId);

        builder
            .Entity<ElectorDao>()
            .HasOne(e => e.Constituencies)
            .WithMany(c => c.Electors)
            .HasForeignKey(e => e.ConstituencyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seeding
        builder.Entity<ConstituencyDao>().HasData(ConstituencyData.Constituencies);
    }
}
