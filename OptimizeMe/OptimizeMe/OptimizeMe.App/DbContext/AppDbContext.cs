using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OptimizeMe.App.Entities;

namespace OptimizeMe.App.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<Author> Authors { get; set; }

    public DbSet<Book> Books { get; set; }

    public DbSet<Publisher> Publishers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(
            "Data Source=C:\\Projects\\Practice\\OptimizeMe\\OptimizeMe\\OptimizeMe.App\\main.db"); //.LogTo(Console.WriteLine,LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);
}