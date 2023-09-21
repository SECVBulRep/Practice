using System.Net.Mime;
using IdentityServer4.AspNetIdentity;
using IdServ.IdentityServer;
using IdServ.IdentityServer.Data;
using IdServ.IdentityServer.Infrasctructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services
    .AddDbContext<ApplicationDbContext>(config => { config.UseInMemoryDatabase("Memory"); })
    .AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequireDigit = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.Password.RequiredLength = 2;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Auth/Login";
    config.LogoutPath = "/Auth/Logout";
    config.Cookie.Name = "IdentityServer.Cookies";
});

builder.Services.AddIdentityServer(config =>
    {
        //config.UserInteraction.LoginUrl = "/Auth/Login";
    })
    .AddAspNetIdentity<IdentityUser>()
    .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
    .AddInMemoryClients(Configuration.GetClients())
    .AddInMemoryApiResources(Configuration.GetApiResources())
    .AddInMemoryApiScopes(Configuration.GetApiScopes())
    .AddProfileService<ProfileService>()
    .AddDeveloperSigningCredential();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    DatabaseInitializer.Init(scope.ServiceProvider);
}

app.Run();