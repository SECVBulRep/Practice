using Microsoft.EntityFrameworkCore;
using WM.Microservices.Seedwork;
using WM.Microservices.Shop.Api.Models;

namespace WM.Microservices.Shop.Api.Repository;

public class ProductRepository: GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ShopContext context) : base(context)
    {
    }
}