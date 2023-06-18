using System.Data;
using Bogus;
using EfVsDapper.App;

public class CompanyGenerator
{
    private readonly IDbConnection _dbConnection;
    private readonly List<int> _ids = new();

    private readonly Faker<Company> _companyGenerator = new Faker<Company>()
        .RuleFor(m => m.Id, f => f.UniqueIndex + 1)
        .RuleFor(m => m.Name, f => f.Company.CompanyName())
        .RuleFor(m => m.FoundationTime, f => f.Date.Past());

    public CompanyGenerator(IDbConnection dbConnection, Random random)
    {
        Randomizer.Seed = random;
        _dbConnection = dbConnection;
    }

    public async Task GenerateCompanies(int count, CompaniesContext companiesContext)
    {
        var generatedCompanies = _companyGenerator.Generate(count);

        var db = companiesContext;
        foreach (var generatedCompany in generatedCompanies)
        {
            db.Companies.Add(generatedCompany);
            _ids.Add(generatedCompany.Id);
        }

        await db.SaveChangesAsync();
    }

    public async Task GlobalCleanup(CompaniesContext companiesContext)
    {
        var db = companiesContext;
        var companies = db.Companies.Where(x => _ids.Contains(x.Id));
        db.RemoveRange(companies);
        await db.SaveChangesAsync();
    }
}