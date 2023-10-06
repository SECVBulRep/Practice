using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5008";
        options.RequireHttpsMetadata = false;
        options.Audience = "OrdersAPI";
        options.TokenValidationParameters = new TokenValidationParameters
        {
             ValidateAudience = true,
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    });

builder.Services.AddCors(options =>
{
    // this defines a CORS policy called "default"
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("https://localhost:7064")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseRouting();

app.UseCors("default");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });


app.Run();