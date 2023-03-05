using Microsoft.EntityFrameworkCore;
using WM.Microservices.Shop.Api.Models;

namespace WM.Microservices.Shop.Api.Repository;

public class ShopContext : DbContext
{
    
    public DbSet<Product> Products { get; set; }
   
    protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseInMemoryDatabase(databaseName: "ProductsDb");
    }
}