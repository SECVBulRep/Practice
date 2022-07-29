using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Service;

public class OrderDeliveryStateDbContext :
    SagaDbContext
{
    public OrderDeliveryStateDbContext(DbContextOptions<OrderDeliveryStateDbContext> options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderDeliveryStateMap(); }
    }
}