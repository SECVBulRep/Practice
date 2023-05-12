using WM.Microservices.Delivery.Api.Models;
using WM.Microservices.Seedwork;

namespace WM.Microservices.Delivery.Api.Repository;

public class OrderRepository: GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(DeliveryContext context) : base(context)
    {
    }
}