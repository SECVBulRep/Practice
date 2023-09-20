using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdServ.IdentityServer.Infrasctructure;

public class ProfileService : IProfileService
{
    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.DateOfBirth, "01.01.2012")
        };
        context.IssuedClaims.AddRange(claims);
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true; // что бы сработал  GetProfileDataAsync
        return Task.CompletedTask;
    }
}