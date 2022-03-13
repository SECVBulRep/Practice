using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saga.WebApp.Saga.StateMachine;

namespace Saga.WebApp.Saga.SagaRepo;

public class OrderStateMap : 
    SagaClassMap<OrderStateData>
{
    protected override void Configure(EntityTypeBuilder<OrderStateData> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);

        
        entity.Property(x => x.OrderId).HasMaxLength(64);
        entity.Property(x => x.OrderCreationDateTime);
        entity.Property(x => x.OrderCancelDateTime);
        entity.Property(x => x.PaymentCardNumber);
        entity.Property(x => x.ProductName);

        // If using Optimistic concurrency, otherwise remove this property
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}


public class OrderStateDbContext : 
    SagaDbContext
{
    public OrderStateDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}


/*
 *public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
   
    public DateTime? OrderCreationDateTime { get; set; }
    
    public DateTime? OrderCancelDateTime { get; set; }
    public Guid OrderId { get; set; }
    public string PaymentCardNumber { get; set; }
    public string ProductName { get; set; }
    
    public byte[] RowVersion { get; set; }
 *
 * 
 */