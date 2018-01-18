using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Models
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public static async Task CreateAdminAccount(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var name = configuration["Data:AdminUser:Name"];
            var email = configuration["Data:AdminUser:Email"];
            var password = configuration["Data:AdminUser:Password"];
            var roleName = configuration["Data:AdminUser:Role"];
            if (await userManager.FindByNameAsync(name) == null)
            {
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                var adminUser = new AppUser
                {
                    UserName = name,
                    Email = email
                };
                var result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, roleName);
                }
            }
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