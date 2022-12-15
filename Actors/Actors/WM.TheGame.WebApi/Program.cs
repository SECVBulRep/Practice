using Orleans.Configuration;
using WM.TheGame.Contracts.Implementations.Chat;
using WM.TheGame.WebApi.Services;

Thread.Sleep(3000);

var builder = WebApplication.CreateBuilder(args);

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json", optional: true);

var configuration = configurationBuilder.Build();
var connectionString = configuration.GetConnectionString("service");


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOrleansClient(config =>
{
    config.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "WM.Cluster";
            options.ServiceId = "Wm.Service";
        })
        .UseAdoNetClustering(options =>
        {
            options.ConnectionString = connectionString;
            options.Invariant = "System.Data.SqlClient";
        })
        .Configure<ConnectionOptions>(options =>
        {
            options.OpenConnectionTimeout = TimeSpan.FromSeconds(60);
            
        });
});


builder.Services.AddSingleton(provider =>
{
    Chat c = new Chat();
    return c;
});

builder.Services.AddHostedService<GameService>();
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