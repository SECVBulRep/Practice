using Bogus;
using EF.Test.App.DbContext;
using EF.Test.App.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF.Test.App;

public class CompanyGenerator
{
    private readonly List<int> _ids = new List<int>();

    private readonly Faker<Role> _roleGenerator = new Faker<Role>()
        .RuleFor(m => m.Id, f => f.UniqueIndex + 1)
        .RuleFor(m => m.Name, f => f.Name.JobType());

    private readonly Faker<User> _userGenerator = new Faker<User>()
        .RuleFor(m => m.Id, f => f.UniqueIndex + 1)
        .RuleFor(m => m.Email, f => f.Internet.Email())
        .RuleFor(m => m.FirstName, f => f.Name.FirstName())
        .RuleFor(m => m.LastName, f => f.Name.LastName())
        .RuleFor(m => m.Created,
            f => f.Date.Recent())
        .RuleFor(m => m.UserName,
            f => f.Internet.UserName())
        .RuleFor(m => m.EmailConfirmed, f => f.Random.Bool())
        .RuleFor(m => m.LastActivity,
            f => f.Date.Recent());

    private readonly Faker<UserRole> _userRoleGenerator =
        new Faker<UserRole>()
            .RuleFor(m => m.Id,
                f => f.UniqueIndex + 1);

    private readonly Faker<Author> _authorGenerator = new Faker<Author>()
        .RuleFor(m => m.Id, f => f.UniqueIndex + 1)
        .RuleFor(m => m.Age, f => Random.Shared.Next(25, 30))
        .RuleFor<string>(m => m.Country,
            f => f.PickRandom(new List<string>{"Russia","Serbia","France"}))
        .RuleFor<string>(
            m => m.NickName, f => f.Internet.UserName());

    private readonly Faker<Publisher> _publisherGenerator = new Faker<Publisher>()
        .RuleFor(m => m.Id, f => f.UniqueIndex + 1)
        .RuleFor(m => m.Established,
            f => f.Date.Past())
        .RuleFor<string>(
            m => m.Name, f => f.Company.CompanyName());

    private readonly Faker<Book> _bookGenerator = new Faker<Book>()
        .RuleFor(m => m.Id, f => f.UniqueIndex + 1)
        .RuleFor(m => m.Published,
            f => f.Date.Between(DateTime.Now.AddYears(-3), DateTime.Now))
        .RuleFor<string>(m => m.ISBN,
            f => f.Internet.Protocol())
        .RuleFor<string>(
            m => m.Name, f => f.Company.CompanyName());

    public CompanyGenerator(Random random) => Randomizer.Seed = random;

    public async Task Generate(int count, AppDbContext context)
    {
        await this.GlobalCleanup(context);
        List<Role> generatedRoles = this._roleGenerator.Generate(count);
        List<User> generatedUsers = this._userGenerator.Generate(count * 5);
        List<Author> generatedAuthors = this._authorGenerator.Generate(count * 2);
        List<Publisher> generatedpublishers = this._publisherGenerator.Generate(10);
        List<Book> generatedBooks = this._bookGenerator.Generate(count * 10);
        foreach (User generatedUser in generatedUsers)
        {
            List<UserRole> userRoles = this._userRoleGenerator.Generate(2);
            userRoles[0].RoleId = generatedRoles[Random.Shared.Next(generatedRoles.Count - 1)].Id;
            userRoles[0].UserId = generatedRoles[Random.Shared.Next(generatedRoles.Count - 1)].Id;
            userRoles[1].RoleId = generatedRoles[Random.Shared.Next(generatedRoles.Count - 1)].Id;
            userRoles[1].UserId = generatedRoles[Random.Shared.Next(generatedRoles.Count - 1)].Id;
            generatedUser.UserRoles = userRoles;
            userRoles = (List<UserRole>)null;
        }

        foreach (Author generatedAuthor in generatedAuthors)
            generatedAuthor.UserId = generatedUsers[Random.Shared.Next(generatedUsers.Count - 1)].Id;
        foreach (Book generatedBook in generatedBooks)
        {
            generatedBook.AuthorId = generatedAuthors[Random.Shared.Next(generatedAuthors.Count - 1)].Id;
            generatedBook.PublisherId = generatedpublishers[Random.Shared.Next(generatedpublishers.Count - 1)].Id;
        }

        AppDbContext db = context;
        await db.Roles.AddRangeAsync((IEnumerable<Role>)generatedRoles);
        await db.Users.AddRangeAsync((IEnumerable<User>)generatedUsers);
        await db.Publishers.AddRangeAsync((IEnumerable<Publisher>)generatedpublishers);
        await db.Authors.AddRangeAsync((IEnumerable<Author>)generatedAuthors);
        await db.Books.AddRangeAsync((IEnumerable<Book>)generatedBooks);
        foreach (Author author in generatedAuthors)
        {
            Author generatedAuthor = author;
            generatedAuthor.BooksCount =
                generatedBooks.Count<Book>((Func<Book, bool>)(x => x.AuthorId == generatedAuthor.Id));
        }

        int num = await db.SaveChangesAsync();
       
    }

    public async Task GlobalCleanup(AppDbContext context)
    {
        AppDbContext db = context;
        int num1 = await db.UserRoles.ExecuteDeleteAsync();
        int num2 = await db.Roles.ExecuteDeleteAsync();
        int num3 = await db.Books.ExecuteDeleteAsync();
        int num4 = await db.Publishers.ExecuteDeleteAsync();
        int num5 = await db.Authors.ExecuteDeleteAsync();
        int num6 = await db.Users.ExecuteDeleteAsync();
        int num7 = await db.SaveChangesAsync();
       
    }
}