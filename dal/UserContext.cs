using backend_devops_rejsekort_v2.dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend_devops_rejsekort_v2.dal
{
    public class UserContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationPair> LocationPairs { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<LocationPair>()
                .HasOne(lp => lp.User)
                .WithMany(u => u.LocationPairs)
                .HasForeignKey(lp => lp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LocationPair>()
                .HasOne(lp => lp.SignInLocation)
                .WithMany()
                .HasForeignKey("SignInLocationId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LocationPair>()
                .HasOne(lp => lp.SignOutLocation)
                .WithMany()
                .HasForeignKey("SignOutLocationId")
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}