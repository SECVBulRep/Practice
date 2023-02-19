using BookShop.WebService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.WebService.Repository;

public class ShopContext : DbContext
{
    
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseInMemoryDatabase(databaseName: "AuthorDb");
    }
}