using Microsoft.AspNetCore.Authentication.Cookies;
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
        
    });

builder.Services.AddHttpClient();


var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();