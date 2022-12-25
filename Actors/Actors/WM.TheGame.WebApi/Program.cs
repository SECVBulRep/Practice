using Orleans.Configuration;
using WM.TheGame.Contracts.Implementations.Chat;
using WM.TheGame.WebApi.Services;

Thread.Sleep(5000);

const int maxAttempts = 50;
var attempt = 0;


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
    config.UseConnectionRetryFilter(RetryFilter);
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
        .Configure<ConnectionOptions>(options => { options.OpenConnectionTimeout = TimeSpan.FromSeconds(60); });
    config.UseTransactions();
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


async Task<bool> RetryFilter(Exception exception, CancellationToken cancellationToken)
{
    attempt++;
    Console.WriteLine($"Cluster client attempt {attempt} of {maxAttempts} failed to connect to cluster.  Exception: {exception}");
    if (attempt > maxAttempts)
    {
        return false;
    }
    await Task.Delay(TimeSpan.FromSeconds(4), cancellationToken);
    return true;
}
