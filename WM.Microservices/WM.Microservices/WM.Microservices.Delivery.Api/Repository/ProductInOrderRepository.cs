using Microsoft.EntityFrameworkCore;
using WM.Microservices.Delivery.Api.Models;
using WM.Microservices.Seedwork;

namespace WM.Microservices.Delivery.Api.Repository;

public class ProductInOrderRepository : GenericRepository<ProductInOrder>,IProductInOrderRepository
{
    public ProductInOrderRepository(DbContext context) : base(context)
    {
    }
}