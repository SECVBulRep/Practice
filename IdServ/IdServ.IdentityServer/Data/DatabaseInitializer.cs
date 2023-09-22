using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace IdServ.IdentityServer.Data;

public static class DatabaseInitializer
{
    public static void Init(IServiceProvider scopeServiceProvider)
    {
        var userManager = scopeServiceProvider.GetService<UserManager<IdentityUser>>();

        var user = new IdentityUser
        {
            UserName = "User"
        };

        var result = userManager.CreateAsync(user, "123qwe").GetAwaiter().GetResult();
        if (result.Succeeded)
        {
            userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator")).GetAwaiter().GetResult();
            
        }
        
        using (var serviceScope = scopeServiceProvider.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
        }
    }
}