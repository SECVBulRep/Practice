using System.Net.Mime;
using System.Reflection;
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

var conString = builder.Configuration.GetConnectionString(nameof(ApplicationDbContext));

builder.Services
    .AddDbContext<ApplicationDbContext>(config =>
    {
        //config.UseInMemoryDatabase("Memory");
        //dotnet ef migrations add InitialCreate
        //dotnet ef database update
        config.UseSqlServer(conString);
    })
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

var migrationsAssembly = typeof(IdentityServerConfiguration).GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddIdentityServer(config =>
    {
        //config.UserInteraction.LoginUrl = "/Auth/Login";
    })
    .AddAspNetIdentity<IdentityUser>()
    //.AddInMemoryIdentityResources(IdentityServerConfiguration.GetIdentityResources())
    //.AddInMemoryClients(IdentityServerConfiguration.GetClients())
    //.AddInMemoryApiResources(IdentityServerConfiguration.GetApiResources())
    //.AddInMemoryApiScopes(IdentityServerConfiguration.GetApiScopes())
    // dotnet ef database drop --context ApplicationDbContext
    //dotnet ef migrations add InitialCreate --context ApplicationDbContext -o Data/ApplicationDb
    //dotnet ef database update --context ApplicationDbContext
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(conString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(conString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
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