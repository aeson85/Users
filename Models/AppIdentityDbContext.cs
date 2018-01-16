using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Users.Models
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser<string>>().ToTable("AspNetUsers").Property(p => p.Id).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityRole<string>>().ToTable("AspNetRoles").Property(p => p.Id).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserLogin<string>>().Property(p => p.ProviderKey).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserLogin<string>>().Property(p => p.LoginProvider).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserToken<string>>().Property(p => p.LoginProvider).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserToken<string>>().Property(p => p.Name).HasMaxLength(256).IsRequired();
        }
    }
}