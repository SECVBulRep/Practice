using WM.Microservices.Delivery.Api.Models;
using WM.Microservices.Seedwork;

namespace WM.Microservices.Delivery.Api.Repository;

public class ProductRepository: GenericRepository<Product>, IProductRepository
{
    public ProductRepository(DeliveryContext context) : base(context)
    {
    }
}