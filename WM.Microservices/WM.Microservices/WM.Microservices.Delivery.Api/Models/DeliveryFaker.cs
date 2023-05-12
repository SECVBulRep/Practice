using Bogus;

namespace WM.Microservices.Delivery.Api.Models;

public class DeliveryFaker
{
    private IWebHostEnvironment _hostEnvironment;

    public DeliveryFaker(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public Faker<Order> GetOrderGenerator()
    {
        return new Faker<Order>()
            .RuleFor(v => v.Description, f => f.Lorem.Text())
            .RuleFor(v => v.Id, f => f.IndexGlobal + 1);
    }

    public List<Order> InitData()
    {
        var bg = GetOrderGenerator();

        var products = new List<Order>();

        for (int i = 0; i < 5; i++)
        {
            var product = bg.Generate();
            products.Add(product);
        }

        return products;
    }
}