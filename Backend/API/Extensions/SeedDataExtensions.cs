using Core.Constants;
using Core.Entities;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class SeedDataExtensions
    {
        public static void SeedIdentityServerAndRoles(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients(configuration))
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiScopes.Any())
                {
                    foreach (var scopeItem in Config.GetApiScopes(configuration))
                    {
                        context.ApiScopes.Add(scopeItem.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources(configuration))
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                var roleManager = scope.ServiceProvider.GetRequiredService<
                    RoleManager<IdentityRole>
                >();
                foreach (var role in RoleConstants.Roles)
                {
                    if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                    }
                }

                var userManager = scope.ServiceProvider.GetRequiredService<
                    UserManager<ApplicationUser>
                >();
                var superAdminEmail = "superadmin@example.com";
                var superAdminUser = userManager
                    .FindByEmailAsync(superAdminEmail)
                    .GetAwaiter()
                    .GetResult();
                if (superAdminUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = "superadmin",
                        Email = superAdminEmail,
                        EmailConfirmed = true,
                    };
                    var result = userManager
                        .CreateAsync(user, "SuperAdmin#123")
                        .GetAwaiter()
                        .GetResult();
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
                    }
                    // คุณสามารถเพิ่ม role อื่นๆ หรือ property อื่นๆ ได้ที่นี่
                }
            }
        }
    }
}
