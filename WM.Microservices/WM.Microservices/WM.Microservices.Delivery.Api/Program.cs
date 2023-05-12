using Microsoft.EntityFrameworkCore;
using WM.Microservices.Delivery.Api.Models;
using WM.Microservices.Delivery.Api.Repository;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<DeliveryFaker>();
Console.WriteLine($"--> UseInMemoryDatabase");
builder.Services.AddDbContext<DeliveryContext>(options => { options.UseInMemoryDatabase(databaseName: "DeliveryDb"); });

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

PrepareData.PopulateAction(app, builder.Environment);

app.Run();

public static class PrepareData
{
    public static void PopulateAction(IApplicationBuilder builder, IWebHostEnvironment webHostEnvironment)
    {
        using (var serviceScope = builder.ApplicationServices.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetService<DeliveryContext>();

            Console.WriteLine("--> Seed data...");
            var orderRepository = serviceScope.ServiceProvider.GetService<IOrderRepository>();

            var deliveryFaker = serviceScope.ServiceProvider.GetService<DeliveryFaker>();

            var data = deliveryFaker!.InitData();

            foreach (var order in data)
            {
                orderRepository?.Create(order);
            }

            Console.WriteLine("--> Seed data is done.");
        }
    }
}