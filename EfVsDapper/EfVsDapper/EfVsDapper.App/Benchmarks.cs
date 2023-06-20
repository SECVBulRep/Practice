using System.Data;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Dapper;
using EfVsDapper.App;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class Benchmarks
{
    private CompaniesContext _companiesContext = null!;
    private IDbConnection _dbConnection = null!;
    private Random _random = null!;
    private Company _testCompany = null!;
    private CompanyGenerator _companyGenerator = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        _random = new Random(420);

        _companiesContext = new CompaniesContext();
        _dbConnection = await (new DapperDataContext()).CreateConnection();
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


    [Benchmark()]
    public async Task<Company> EF_Find()
    {
        return (await _companiesContext.Companies.FindAsync(_testCompany.Id))! ;
    }

    [Benchmark()]
    public async Task<Company> EF_Single()
    {
        return  _companiesContext.Companies.SingleOrDefault(x=>x.Id==_testCompany.Id)! ;
    }
    
    [Benchmark()]
    public async Task<Company> EF_First()
    {
        return  _companiesContext.Companies.FirstOrDefault(x=>x.Id==_testCompany.Id)! ;
    }
    
    
    [Benchmark()]
    public async Task<Company> Dapper_GetById()
    {
        return  _dbConnection.QuerySingleOrDefault<Company>("SELECT * FROM COMPANIES WHERE Id=@Id LIMIT 1",new {_testCompany.Id}) ;
    }

}

public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig()
    {
        AddJob(Job.MediumRun
            .WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}