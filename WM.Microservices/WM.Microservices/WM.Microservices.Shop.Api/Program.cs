using Microsoft.EntityFrameworkCore;
using WM.Microservices.Shop.Api.Models;
using WM.Microservices.Shop.Api.Repository;
using WM.Microservices.Shop.Api.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DbContext, ShopContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddHttpClient<IDeliveryDataClient,HttpDeliveryDataClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

PrepareData.PopulateAction(app);
Console.WriteLine($"--> delivery endpoint {builder.Configuration["deliverySystem"]}");

app.Run();


public static class PrepareData
{
    public static void PopulateAction(IApplicationBuilder builder)
    {
        using (var serviceScope = builder.ApplicationServices.CreateScope())
        {
            Console.WriteLine("--> Seed data...");
            var productRepository = serviceScope.ServiceProvider.GetService<IProductRepository>();

            var data = ShopFaker.InitData();

            foreach (var product in data)
            {
                productRepository?.Create(product);
            }
            Console.WriteLine("--> Seed data is done.");
        }
    }

    
}