using MassTransit;
using MassTransit.Saga;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Saga.WebApp.Infra;
using Saga.WebApp.RB.Consumers;
using Saga.WebApp.Saga.StateMachine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
});

builder.Services.AddDbContext<OrderDbContext>(o => { o.UseSqlServer(); });
builder.Services.AddScoped<IOrderDataAccess, OrderDataAccess>();

var saga = new OrderStateMachine();
var repo = new InMemorySagaRepository<OrderStateData>();


builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("172.16.29.35", "NPD", h =>
        {
            h.Username("bulat");
            h.Password("123qweASD");
        });
        
        cfg.ReceiveEndpoint("validate-order-queue", e =>
        {
            e.Consumer<CardNumberValidateConsumer>();
        });

        cfg.ReceiveEndpoint("saga-bus-queue", e =>
        {
            e.StateMachineSaga(saga,repo);
        });

        cfg.ConfigureEndpoints(context);
    });
    
    

});
builder.Services.AddMassTransitHostedService();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();