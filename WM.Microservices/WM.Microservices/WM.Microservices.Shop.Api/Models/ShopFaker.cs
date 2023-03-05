using Bogus;

namespace WM.Microservices.Shop.Api.Models;

public class ShopFaker
{
    public static Faker<Product> GetProductGenerator()
    {
        return new Faker<Product>()
            .RuleFor(v => v.Name, f => f.Commerce.ProductName())
            .RuleFor(v => v.Description, f => f.Commerce.ProductDescription())
            .RuleFor(v=>v.Price, f=>f.Random.Decimal())
            .RuleFor(v => v.Id, f => f.IndexGlobal+1);
    }

    public static List<Product> InitData()
    {
        var bg = GetProductGenerator();

        var products = new List<Product>();

        for (int i = 0; i < 5; i++)
        {
            var product = bg.Generate();
            products.Add(product);
        }

        return products;
    }
}