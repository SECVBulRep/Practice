// See https://aka.ms/new-console-template for more information

using System.Data;
using BenchmarkDotNet.Running;
using Dapper;
using EfVsDapper.App;
using Microsoft.EntityFrameworkCore;

//BenchmarkRunner.Run<Benchmarks>();


CompaniesContext _companiesContext = null!;
IDbConnection _dbConnection = null!;
Random _random = null!;
Company _testCompany = null!;
CompanyGenerator _companyGenerator = null!;


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

await _companyGenerator.GlobalCleanup(_companiesContext);
await _dbConnection.ExecuteAsync("""
             DELETE FROM COMPANIES WHERE Id= @Id
             """, _testCompany);