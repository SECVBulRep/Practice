using Microsoft.EntityFrameworkCore;
using WM.Microservices.Shop.Api.Models;

namespace WM.Microservices.Shop.Api.Repository;

public class ShopContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ShopContext(DbContextOptions options) : base(options)
    {
    }
}