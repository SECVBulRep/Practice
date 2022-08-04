using Delivery.Components.StateMachines;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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