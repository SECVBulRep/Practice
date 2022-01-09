using Microsoft.EntityFrameworkCore;
using Saga.WebApp.Model;

namespace Saga.WebApp.Infra;

public class OrderDbContext : DbContext
{
    public DbSet<OrderModel> OrderData { get; set; }

    public OrderDbContext()
    {
    }

    public OrderDbContext(DbContextOptions
        <OrderDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=172.16.29.35; initial catalog=Practices;Persist Security Info=True;User ID=bulat_f;Password=qwerty");
    }
}