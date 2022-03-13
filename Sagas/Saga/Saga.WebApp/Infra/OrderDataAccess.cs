using Saga.WebApp.Model;

namespace Saga.WebApp.Infra;

public class OrderDataAccess : IOrderDataAccess
{
    public List<OrderModel> GetAllOrder()
    {
        using (var context = new OrderDbContext())
        {
            return context.OrderData.ToList();
        }
    }
    public void SaveOrder(OrderModel order)
    {
        using (var context = new OrderDbContext())
        {
            context.Add<OrderModel>(order);
            context.SaveChanges();
        }
    }

    public OrderModel GetOrder(Guid orderId)
    {
        using (var context = new OrderDbContext())
        {
            return context.OrderData.SingleOrDefault(x => x.OrderId == orderId);
        }
    }

    public bool DeleteOrder(Guid orderId)
    {
        using (var context = new OrderDbContext())
        {
           var order =  context.OrderData.SingleOrDefault(x => x.OrderId == orderId);

           if (order != null)
           {
               context.Remove(order);
               context.SaveChanges();
               return true;
           }
           else
           {
               return false;
           }
        }
    }
}