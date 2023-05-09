using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WM.Microservices.Shop.Api.AsyncDataServices;
using WM.Microservices.Shop.Api.Dtos;
using WM.Microservices.Shop.Api.Models;
using WM.Microservices.Shop.Api.Repository;
using WM.Microservices.Shop.Api.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(_ => builder.Environment);
builder.Services.AddSingleton<ShopFaker>();

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine($"--> UseInMemoryDatabase");
    builder.Services.AddDbContext<ShopContext>(options => { options.UseInMemoryDatabase(databaseName: "ProductsDb"); });
}
else
{
    Console.WriteLine($"--> Use SQL SERVER Database");
    builder.Services.AddDbContext<ShopContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("shop"));
    });
}

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddHttpClient<IDeliveryDataClient, HttpDeliveryDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

PrepareData.PopulateAction(app, builder.Environment);
Console.WriteLine($"--> delivery endpoint {builder.Configuration["deliverySystem"]}");

app.Run();


public static class PrepareData
{
    public static void PopulateAction(IApplicationBuilder builder, IWebHostEnvironment webHostEnvironment)
    {
        using (var serviceScope = builder.ApplicationServices.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetService<ShopContext>();
            var messageBusClient = serviceScope.ServiceProvider.GetService<IMessageBusClient>();

            if (webHostEnvironment.IsProduction())
            {
                Console.WriteLine($"--> apllying migrations ...");

                try
                {
                    dbContext!.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }


            Console.WriteLine("--> Seed data...");
            var productRepository = serviceScope.ServiceProvider.GetService<IProductRepository>();
            var shopFaker = serviceScope.ServiceProvider.GetService<ShopFaker>();
            var mapper = serviceScope.ServiceProvider.GetService<IMapper>();
            
            
            

            var data = shopFaker!.InitData();

            foreach (var product in data)
            {
                productRepository?.Create(product);
             
                
                try
                {
                    var ret = mapper!.Map<ProductReadDto>(product);
                    var message = mapper!.Map<ProductPublishedDto>(ret);
                    message.Event = "Product_Published";
                    messageBusClient!.PublishNewProduct(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
            
                }
                
            }

            Console.WriteLine("--> Seed data is done.");
        }
    }
}