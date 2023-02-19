using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using BookShop.WebService.Models;
using BookShop.WebService.Repository;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy=JsonNamingPolicy.CamelCase;
});


builder.Services.AddScoped<DbContext, ShopContext>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();

PrepData.PopulateAction(app);

app.Run();


public static class PrepData
{
    public static void PopulateAction(IApplicationBuilder builder)
    {
        using (var serviceScope = builder.ApplicationServices.CreateScope())
        {
            var authorRepository = serviceScope.ServiceProvider.GetService<IAuthorRepository>();

            var data = BookShopFaker.InitData();

            foreach (var author in data)
            {
                authorRepository.Create(author);
            }
        }
    }

    
}