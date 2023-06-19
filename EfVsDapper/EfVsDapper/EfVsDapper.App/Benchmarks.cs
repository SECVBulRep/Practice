using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using EfVsDapper.App;

[MemoryDiagnoser()]
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





}