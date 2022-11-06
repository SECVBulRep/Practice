using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Polly;
using WM.TheGame.Contracts.Contracts;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Implementations.Chat;
using WM.TheGame.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json", optional: true);

var configuration = configurationBuilder.Build();
var connectionString = configuration.GetConnectionString("service");


builder.Services.AddSingleton(provider =>
{
    Chat c = new Chat();
    return c;
});

builder.Services.AddHostedService<GameService>();

builder.Services.AddSingleton(provider  =>
{
    return Policy<IClusterClient>
        .Handle<Exception>()
        .WaitAndRetry(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3),
            TimeSpan.FromSeconds(5)
        })
        .Execute( () =>
        {
            var client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "WM.Cluster";
                    options.ServiceId = "Wm.Service";
                })
                .UseAdoNetClustering(options =>
                {
                    options.ConnectionString = connectionString;
                    options.Invariant = "System.Data.SqlClient";
                })
                .ConfigureApplicationParts(part =>
                {
                    part.AddApplicationPart(typeof(IGameGrain).Assembly).WithReferences();
                })
                .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning).AddConsole())
                .Build();

             client.Connect().Wait();
             return client;
        });
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