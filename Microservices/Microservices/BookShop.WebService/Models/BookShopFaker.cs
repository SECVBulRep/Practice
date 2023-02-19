using Bogus;

namespace BookShop.WebService.Models;

public class BookShopFaker
{
    public static Faker<Book> GetBookGenerator()
    {
        return new Faker<Book>()
            .RuleFor(v => v.Title, f => f.Company.CatchPhrase())
            .RuleFor(v => v.Id, f => f.UniqueIndex);
    }
    
    
    public static Faker<Author> GetAuthorGenerator()
    {
        return new Faker<Author>()
            .RuleFor(v => v.Id, f => f.UniqueIndex)
            .RuleFor(v => v.FirstName, f => f.Name.FirstName())
            .RuleFor(v => v.LastName, f => f.Name.LastName());

    }


    public static List<Author> InitData()
    {
        var bg = BookShopFaker.GetBookGenerator();
        bg.Generate();
        var ag = BookShopFaker.GetAuthorGenerator();
        ag.Generate();
        var authors = ag.Generate(10);
        Random rd = new Random();
        foreach (var author in authors)
        {
            var books = bg.Generate(rd.Next(1, 3));
            author.Books = books;
        }

        return authors;
    }
}