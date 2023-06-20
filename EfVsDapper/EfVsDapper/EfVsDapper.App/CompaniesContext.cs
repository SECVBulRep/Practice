using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
    private SqliteConnection _connection;

    public async Task<IDbConnection> CreateConnection()
    {
        _connection =
            new SqliteConnection(
                $"""Data Source=D:\WORK\Practice\Practice\EfVsDapper\EfVsDapper\EfVsDapper.App\Companies.db""");
        await _connection.OpenAsync();
        return _connection;
    }


    public async Task<Company> Find(int id)
    {
        var cacheKey = $"MyEntity_{id}";
        var entity = CacheManager.Get<Company>(cacheKey);
        if (entity == null)
        {
            entity = await FindExt(id)!;
            CacheManager.Set(cacheKey, entity);
        }

        return entity;
    }

    private Task<Company>? FindExt(int id)
    {
        var query = "SELECT * FROM Companies WHERE Id = @Id LIMIT 1";
        var parameters = new { Id = id };
        var entity = _connection.QuerySingleOrDefaultAsync<Company>(query, parameters);


        // Проверяем, загружен ли объект
        if (entity != null)
        {
            // Помещаем объект в контекст отслеживания (подобно EF)
            _connection.TrackEntity(entity);
        }

        return entity;
    }
}