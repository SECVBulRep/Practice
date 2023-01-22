// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Bogus;

Console.WriteLine("Hello, World!");

var customerFaker = new Faker<Customer>("it")
    .RuleFor(x => x.Name, y => y.Person.FullName)
    .RuleFor(x => x.Phone, y => y.Phone.PhoneNumber())
    .RuleFor(x => x.City, y => y.Address.City())
    .RuleFor(x => x.Email, y => y.Person.Email);


var customer = customerFaker.Generate();

foreach (var data in customerFaker.GenerateForever())
{
    var json = JsonSerializer.Serialize(data);

    await Task.Delay(1000);
    Console.WriteLine(json);
}


public class Customer
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string City { get; set; }
    public string Email { get; set; }
}