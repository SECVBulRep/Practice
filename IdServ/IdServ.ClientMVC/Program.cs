using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(config =>
    {
        config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, config =>
    {
        config.Authority = "https://localhost:5008";
        config.ClientId = "client_id_mvc";
        config.ClientSecret = "client_secret_mvc";
        config.ResponseType = OpenIdConnectResponseType.Code;
        config.Scope.Add("openid");
        config.Scope.Add("profile");
        config.Scope.Add("OrdersAPI");
        config.Scope.Add("offline_access");

        

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

    // config.AddPolicy("OlderThan", builder =>
    // {
    //     builder.AddRequirements(new OlderThanRequirement(10));
    // });
});

builder.Services.AddSingleton<IAuthorizationHandler, OlderThanRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _options = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policyExist = await base.GetPolicyAsync(policyName);

        if (policyExist == null)
        {
            policyExist = new AuthorizationPolicyBuilder().AddRequirements(new OlderThanRequirement(10)).Build();
            _options.AddPolicy(policyName, policyExist);
        }

        return policyExist;
    }
}


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
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OlderThanRequirement requirement)
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