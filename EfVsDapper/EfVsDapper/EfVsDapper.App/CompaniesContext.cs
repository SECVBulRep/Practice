using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EfVsDapper.App;

public class CompaniesContext : DbContext
{
    public DbSet<Company> Companies { get; set; }

    private IDbConnection _dbConnection;

    public CompaniesContext(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       // optionsBuilder.UseSqlite($"""Data Source=C:\Projects\Practice\EfVsDapper\EfVsDapper\EfVsDapper.App\Companies.db""");
       optionsBuilder.UseSqlite((DbConnection)_dbConnection);
    }
}
public class DapperDataContext
{
    public async Task<IDbConnection> CreateConnection()
    {
        var connection =  new SqliteConnection($"""Data Source=C:\Projects\Practice\EfVsDapper\EfVsDapper\EfVsDapper.App\Companies.db""");
        await connection.OpenAsync();
        return connection;
    }
}