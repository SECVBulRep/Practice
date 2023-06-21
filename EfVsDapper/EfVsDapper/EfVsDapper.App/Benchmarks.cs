using System.Data;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Dapper;
using EfVsDapper.App;
using Microsoft.EntityFrameworkCore;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class Benchmarks
{
    private CompaniesContext _companiesContext = null!;
    private IDbConnection _dbConnection = null!;
    private Random _random = null!;
    private Company _testCompany = null!;
    private CompanyGenerator _companyGenerator = null!;
    private DapperDataContext _dapperContext;

    [GlobalSetup]
    public async Task Setup()
    {
        _random = new Random(420);

        _dapperContext = new DapperDataContext();
        _dbConnection = await _dapperContext.CreateConnection();
        _companiesContext = new CompaniesContext(_dbConnection);
        _companyGenerator = new CompanyGenerator(_dbConnection, _random);

        await _companyGenerator.GenerateCompanies(100, _companiesContext);

        _testCompany = new Company()
        {
            Id = 1000,
            Name = "IqSoft",
            FoundationTime = DateTime.Now.AddYears(-9)
        };

        await _dbConnection.ExecuteAsync("""
            INSERT INTO COMPANIES(Id,Name,FoundationTime)
            VALUES (@Id,@Name,@FoundationTime)
""", _testCompany);
    }


    [GlobalCleanup]
    public async Task CleanUp()
    {
        await _companyGenerator.GlobalCleanup(_companiesContext);
        await _dbConnection.ExecuteAsync("""
             DELETE FROM COMPANIES WHERE Id= @Id
             """, _testCompany);
    }

    // [Benchmark()]
    // public async Task<List<Company>> EF_Filter()
    // {
    //     return _companiesContext.Companies.Where(x => x.Name == _testCompany.Name).ToList();
    // }
    //
    // private static readonly Func<CompaniesContext, string, IAsyncEnumerable<Company>> GetCompaniesAsync =
    //     EF.CompileAsyncQuery(
    //         (CompaniesContext context, string name) => context.Companies.Where(x => x.Name == name)
    //     );
    //
    // [Benchmark()]
    // public async Task<List<Company>> EF_Filter_Compiled()
    // {
    //     var list = new List<Company>();
    //
    //     await foreach (var item in GetCompaniesAsync(_companiesContext, _testCompany.Name))
    //     {
    //         list.Add(item);
    //     }
    //     return list;
    // }
    //
    //
    // [Benchmark()]
    // public async Task<List<Company>> Dapper_Filter()
    // {
    //     var result = await _dbConnection.QueryAsync<Company>("SELECT * FROM COMPANIES WHERE Name=@Name",
    //         new {_testCompany.Name});
    //
    //     return result.ToList();
    // }


    //
    // [Benchmark()]
    // public async Task<Company> EF_Find()
    // {
    //     return (await _companiesContext.Companies.FindAsync(_testCompany.Id))!;
    // }
    //
    //  
    // [Benchmark()]
    // public async Task<Company> Dapper_Find()
    // {
    //     return await _dapperContext.Find(_testCompany.Id)!;
    // }


    // [Benchmark()]
    // public async Task<Company> EF_Single()
    // {
    //     return _companiesContext.Companies.AsNoTracking().SingleOrDefault(x => x.Id == _testCompany.Id)!;
    // }
    //
    // private static readonly Func<CompaniesContext, int, Task<Company?>> SingleCompanyAsync =
    //     EF.CompileAsyncQuery((CompaniesContext context, int id) => context
    //         .Companies.SingleOrDefault(x => x.Id == id));
    //
    // [Benchmark()]
    // public async Task<Company> EF_Single_Compiled()
    // {
    //     return (await SingleCompanyAsync(_companiesContext,_testCompany.Id))!;
    // }

    // [Benchmark()]
    // public async Task<Company> EF_First()
    // {
    //     return _companiesContext.Companies.FirstOrDefault(x => x.Id == _testCompany.Id)!;
    // }
    //
    // private static readonly Func<CompaniesContext, int, Task<Company?>> FirstCompanyAsync =
    //     EF.CompileAsyncQuery((CompaniesContext context, int id) => context
    //         .Companies.FirstOrDefault(x => x.Id == id));
    //
    // [Benchmark()]
    // public async Task<Company> EF_First_Compiled()
    // {
    //     return (await FirstCompanyAsync(_companiesContext,_testCompany.Id))!;
    // }
    //


    [Benchmark()]
    public async Task<Company> EF_Query_GetById()
    {
        return await _companiesContext.Database
            .SqlQuery<Company>($"SELECT * FROM COMPANIES WHERE Id={_testCompany.Id} LIMIT 1")
            .SingleOrDefaultAsync();
    }


    [Benchmark()]
    public async Task<Company> Dapper_GetById()
    {
        return _dbConnection.QuerySingleOrDefault<Company>("SELECT * FROM COMPANIES WHERE Id=@Id LIMIT 1",
            new { _testCompany.Id });
    }

//
//     [Benchmark()]
//     public async Task<Company> EF_Add_Delete()
//     {
//         var company = new Company()
//         {
//             Id = 1001,
//             Name = "WebMoney",
//             FoundationTime = DateTime.Now.AddYears(-9)
//         };
//
//         await _companiesContext.Companies.AddAsync(company);
//         await _companiesContext.SaveChangesAsync();
//
//         _companiesContext.Remove(company);
//         await _companiesContext.SaveChangesAsync();
//         return company;
//     }
//
//     [Benchmark()]
//     public async Task<Company> Dapper_Add_Delete()
//     {
//         var company = new Company()
//         {
//             Id = 1001,
//             Name = "WebMoney",
//             FoundationTime = DateTime.Now.AddYears(-9)
//         };
//
//
//         await _dbConnection.ExecuteAsync("""
// INSERT INTO Companies (Id,Name,FoundationTime)
// VALUES (@Id,@Name,@FoundationTime)
//
// """, company);
//
//
//         await _dbConnection.ExecuteAsync("""
// DELETE FROM Companies WHERE Id =@Id
// """, company);
//
//         return company;
//     }
}

public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig()
    {
        AddJob(Job.MediumRun
            .WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}