﻿// See https://aka.ms/new-console-template for more information


using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ExpressMapper.Extensions;


ExpressMapper.Mapper.Register<Customer,CustomerDto>();



BenchmarkRunner.Run<MapperTester>();


public class MapperTester
{

    public static Customer Customer = new Customer
    {
        Age = 13,
        BirthDate = DateTime.Now.Date,
        FirstName = "Alex",
        Contacts = new List<Contact>
        {
            new Contact {Type = ContactType.Email, Value = "sdsdf@sdf.ru"},
            new Contact {Type = ContactType.Email, Value = "sdsdf@sdf.ru"}
        }
    };

    public static IMapper AutoMapper =(new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Customer, CustomerDto>();
        cfg.CreateMap<Contact, ContactDto>();
    })).CreateMapper();

    
    
    [Benchmark]
    public CustomerDto ExpressMapperMap()
    {
        return Customer.Map<Customer,CustomerDto>();
    }


    [Benchmark]
    public CustomerDto AutoMapperMap()
    {
        return AutoMapper.Map<CustomerDto>(Customer);
    }


    [Benchmark]
    public CustomerDto ManualMap()
    {
        var customerDto = new CustomerDto
        {
            Age = Customer.Age,
            BirthDate = Customer.BirthDate,
            FirstName = Customer.FirstName,
            Contacts = new List<ContactDto>()
        };

        foreach (var customerContact in Customer.Contacts)
        {
            customerDto.Contacts.Add(new ContactDto
            {
                Type = customerContact.Type,
                Value = customerContact.Value
            });
        }
        return customerDto;
    }
}


public class CustomerDto
{
    public string FirstName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    
    public List<ContactDto> Contacts { get; set; }
}


public class ContactDto
{
    public ContactType Type { get; set; }
    public string Value { get; set; }
}


public class Customer
{
    public string FirstName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public List<Contact> Contacts { get; set; }
}


public class Contact
{
    public ContactType Type { get; set; }
    public string Value { get; set; }
}

public enum ContactType
{
    Phone,
    Email
}