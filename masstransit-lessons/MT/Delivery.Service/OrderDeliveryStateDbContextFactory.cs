using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Delivery.Service;

public class OrderDeliveryStateDbContextFactory :
    IDesignTimeDbContextFactory<OrderDeliveryStateDbContext>
{
    public OrderDeliveryStateDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OrderDeliveryStateDbContext>();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("service"), m =>
        {
            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            m.MigrationsHistoryTable($"__{nameof(OrderDeliveryStateDbContext)}");
        });


        return new OrderDeliveryStateDbContext(optionsBuilder.Options);
    }
}