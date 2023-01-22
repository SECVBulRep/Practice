using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TeastApps.WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);


var key = Encoding.ASCII.GetBytes(AuthOptions.Key);
    
builder.Services.AddAuthentication(options =>
{ }).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        
        IssuerSigningKey = new SymmetricSecurityKey
            (key)
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();