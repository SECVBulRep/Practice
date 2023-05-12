using Microsoft.EntityFrameworkCore;
using WM.Microservices.Delivery.Api.Models;

namespace WM.Microservices.Delivery.Api.Repository;

public class DeliveryContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Order> Orders  { get; set; }

    public DeliveryContext(DbContextOptions options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(x => x.Products)
            .WithOne(p => p.Order)
            .HasForeignKey(x => x.OrderId);

        modelBuilder.Entity<Product>()
            .HasOne(x => x.Order)
            .WithMany(p => p.Products)
            .HasForeignKey(f => f.OrderId);

    }
}