using MassTransit;
using MassTransit.MongoDbIntegration.MessageData;
using MT.SampleContracts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MT.SampleComponents.Consumers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); });

builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    logging.AddConsole();
});

builder.Services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
//для не медиаторов юзать это  метод
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddBus(provider =>
    {
        return Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("localhost", "work", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
           // MessageDataDefaults.Threshold = 5;
            cfg.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1","msgs"));
            
        });
    });

    //cfg.AddRequestClient<ISubmitOrder>(new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}")); // так нельзя делать!!!
    cfg.AddRequestClient<ISubmitOrder>(
        new Uri(
            $"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}")); // так нельзя делать!!!
    
    cfg.AddRequestClient<ICheckOrder>();
    
    //cfg.AddRequestClient<ISubmitOrder>();
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();