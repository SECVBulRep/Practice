using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EfVsDapper.App;

public class CompaniesContext : DbContext
{
    public DbSet<Company> Companies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"""Data Source=D:\WORK\Practice\Practice\EfVsDapper\EfVsDapper\EfVsDapper.App\Companies.db""");
    }
}
public class DapperDataContext
{
    public DapperDataContext()
    {
       
    }

    public async Task<IDbConnection> CreateConnection()
    {
        var connection =  new SqliteConnection($"""Data Source=D:\WORK\Practice\Practice\EfVsDapper\EfVsDapper\EfVsDapper.App\Companies.db""");
        await connection.OpenAsync();
        return connection;
    }
}