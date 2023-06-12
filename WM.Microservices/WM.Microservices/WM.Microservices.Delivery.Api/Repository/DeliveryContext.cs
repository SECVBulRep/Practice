using Microsoft.EntityFrameworkCore;
using WM.Microservices.Delivery.Api.Models;

namespace WM.Microservices.Delivery.Api.Repository;

public class DeliveryContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Order> Orders  { get; set; }
    
    public DbSet<ProductInOrder> ProductInOrders  { get; set; }

    public DeliveryContext(DbContextOptions options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductInOrder>().HasKey(sc => new { sc.OrderId, sc.ProductId });
        
        
        modelBuilder.Entity<ProductInOrder>()
            .HasOne<Order>(sc => sc.Order)
            .WithMany(s => s.ProductsInOrder)
            .HasForeignKey(sc => sc.OrderId);

      //  modelBuilder.Entity<ProductInOrder>().Navigation(x => x.Order).AutoInclude();
        modelBuilder.Entity<ProductInOrder>().Navigation(x => x.Product).AutoInclude();
        modelBuilder.Entity<Order>().Navigation(x => x.ProductsInOrder).AutoInclude();


        // modelBuilder.Entity<ProductInOrder>()
        //     .HasOne<Product>(sc => sc.Product)
        //     .WithMany(s => s.ProductsInOrder)
        //     .HasForeignKey(sc => sc.CId);

        // modelBuilder.Entity<StudentCourse>()
        //     .HasOne<Student>(sc => sc.Student)
        //     .WithMany(s => s.StudentCourses)
        //     .HasForeignKey(sc => sc.SId);
        //
        //
        // modelBuilder.Entity<StudentCourse>()
        //     .HasOne<Course>(sc => sc.Course)
        //     .WithMany(s => s.StudentCourses)
        //     .HasForeignKey(sc => sc.CId);

        // modelBuilder.Entity<Order>()
        //     .HasMany(x => x.Products)
        //     .WithOne(p => p.Order)
        //     .HasForeignKey(x => x.OrderId);
        //
        // modelBuilder.Entity<Product>()
        //     .HasOne(x => x.Order)
        //     .WithMany(p => p.Products)
        //     .HasForeignKey(f => f.OrderId);

    }
}