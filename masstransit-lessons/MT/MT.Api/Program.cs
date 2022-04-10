using MassTransit;
using MT.SampleComponents.Consumers;
using MT.SampleContracts;
using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


/*builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumer<SubmitOrderConsumer>();
    cfg.AddRequestClient<ISubmitOrder>();
});
*/


builder.Services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

//для не медиаторов юзать это  метод
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddBus(provider =>
    {
        return Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("172.16.29.35", "masstransit-test", h =>
            {
                h.Username("bulat");
                h.Password("123qweASD");
            });
        });
    });
    
    cfg.AddRequestClient<ISubmitOrder>();
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