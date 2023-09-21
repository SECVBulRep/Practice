using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(config =>
    {
        config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = "oidc";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect("oidc", config =>
    {
        config.Authority = "https://localhost:5008";
        config.ClientId = "client_id_mvc";
        config.ClientSecret = "client_secret_mvc";
        config.ResponseType = OpenIdConnectResponseType.Code;
        config.Scope.Add("openid");
        config.Scope.Add("profile");
        config.Scope.Add("OrdersAPI");


        config.SaveTokens = true;
        config.RequireHttpsMetadata = false;
        config.GetClaimsFromUserInfoEndpoint = true;


        // config.ClaimActions.MapAll(); плохой вариант
        config.ClaimActions.MapJsonKey(ClaimTypes.DateOfBirth, ClaimTypes.DateOfBirth);
    });

builder.Services.AddHttpClient();

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("HasDateOfBirth", builder => { builder.RequireClaim(ClaimTypes.DateOfBirth); });
    
    config.AddPolicy("OlderThan", builder =>
    {
        builder.AddRequirements(new OlderThanRequirement(10));
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, OlderThanRequirementHandler>();
    
var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



public class OlderThanRequirement : IAuthorizationRequirement
{
    public OlderThanRequirement(int years)
    {
        Years = years;
    }

    public int Years { get; }
}

public class OlderThanRequirementHandler : AuthorizationHandler<OlderThanRequirement>
{
   
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OlderThanRequirement requirement)
    {
        var hasClaim = context.User.HasClaim(x => x.Type == ClaimTypes.DateOfBirth);
        if (!hasClaim)
        {
            return Task.CompletedTask;
        }

        var dateOfBirth = context.User.FindFirst(x => x.Type == ClaimTypes.DateOfBirth).Value;
        var date = DateTime.Parse(dateOfBirth, new CultureInfo("ru-RU"));
        var canEnterDiff = DateTime.Now.Year - date.Year;
        if (canEnterDiff >= requirement.Years)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;

    }
}