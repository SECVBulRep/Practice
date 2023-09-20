using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

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
    }
}