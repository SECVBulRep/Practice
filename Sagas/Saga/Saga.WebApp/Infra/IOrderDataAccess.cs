using Saga.WebApp.Model;

namespace Saga.WebApp.Infra;

public interface IOrderDataAccess
{
    List<OrderModel> GetAllOrder();
    void SaveOrder(OrderModel order);
    OrderModel GetOrder(Guid orderId);
    bool DeleteOrder(Guid orderId);


}