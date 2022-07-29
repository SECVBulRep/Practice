using System.Reflection;
using Delivery.Components.StateMachines;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Delivery.Service;

public class OrderDeliveryStateMap :
    SagaClassMap<OrderDeliveryState>
{
    protected override void Configure(EntityTypeBuilder<OrderDeliveryState> entity, ModelBuilder model)
    {
        entity.Property(x => x.OrderId);
        entity.HasIndex(x => x.OrderId).IsUnique();
        entity.Property(x => x.CurrentState);
    }
}

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