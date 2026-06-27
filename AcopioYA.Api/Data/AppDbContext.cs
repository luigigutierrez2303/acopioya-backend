using AcopioYA.Api.Domain.Entities;
using AcopioYA.Api.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcopioYA.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CollectionCenter> CollectionCenters => Set<CollectionCenter>();
    public DbSet<CenterNeed> CenterNeeds => Set<CenterNeed>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // imprescindible: configura Identity

        builder.Entity<CollectionCenter>(e =>
        {
            e.Property(c => c.Name).IsRequired().HasMaxLength(150);
            e.Property(c => c.Address).IsRequired().HasMaxLength(300);
            e.Property(c => c.State).IsRequired().HasMaxLength(80);
            e.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

            e.HasIndex(c => c.State);
            e.HasIndex(c => c.IsVerified);

            e.HasMany(c => c.Needs)
             .WithOne(n => n.CollectionCenter)
             .HasForeignKey(n => n.CollectionCenterId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CenterNeed>(e =>
        {
            e.Property(n => n.Category).HasConversion<string>().HasMaxLength(20);
            e.Property(n => n.Urgency).HasConversion<string>().HasMaxLength(20);
        });
    }
}
