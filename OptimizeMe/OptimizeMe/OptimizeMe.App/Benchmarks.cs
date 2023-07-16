﻿using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using OptimizeMe.App.DbContext;
using OptimizeMe.App.Dto;
using OptimizeMe.App.Entities;

namespace OptimizeMe.App;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
[HideColumns("Job", "StdDev")]
public class Benchmarks
{
    private AppDbContext _appDbContext;
    private Random _random;
    private CompanyGenerator _generator;

    [GlobalSetup]
    public async Task Setup()
    {
        _random = new Random(420);
        _appDbContext = new AppDbContext();

        _generator = new CompanyGenerator(_random);
        await _generator.Generate(500, _appDbContext);
    }

    /// <summary>
    /// Get top 2 Authors (FirstName, LastName, UserName, Email, Age, Country) 
    /// from country Serbia aged 27, with the highest BooksCount
    /// and all his/her books (Book Name/Title and Publishment Year) published before 1900
    /// </summary>
    /// <returns></returns>
    [Benchmark(Baseline = true)]
    public List<AuthorDTO> GetAuthors()
    {
        using (AppDbContext appDbContext = new AppDbContext())
        {
            List<AuthorDTO> list =
                appDbContext.Authors
                    .Include<Author, User>(x => x.User)
                    .ThenInclude(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .Include(x => x.Books)
                    .ThenInclude(x => x.Publisher)
                    .ToList().Select(x => new AuthorDTO
                    {
                        UserCreated = x.User.Created,
                        UserEmailConfirmed = x.User.EmailConfirmed,
                        UserFirstName = x.User.FirstName,
                        UserLastActivity = x.User.LastActivity,
                        UserLastName = x.User.LastName,
                        UserEmail = x.User.Email,
                        UserName = x.User.UserName,
                        UserId = x.User.Id,
                        RoleId = x.User.UserRoles
                            .FirstOrDefault(y => y.UserId == x.UserId).RoleId,
                        BooksCount = x.BooksCount,
                        AllBooks = x.Books.Select(y => new BookDto
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Published = y.Published,
                            ISBN = y.ISBN,
                            PublisherName = y.Publisher.Name
                        }).ToList(),
                        AuthorAge = x.Age,
                        AuthorCountry = x.Country,
                        AuthorNickName = x.NickName,
                        Id = x.Id
                    }).ToList()
                    .Where(x => x.AuthorCountry == "Serbia" && x.AuthorAge > 26)
                    .ToList().OrderByDescending(x => x.BooksCount)
                    .ToList().Take(2).ToList();
            List<AuthorDTO> authors = new List<AuthorDTO>();
            foreach (AuthorDTO authorDto in list)
            {
                List<BookDto> bookDtoList = new List<BookDto>();
                foreach (BookDto allBook in authorDto.AllBooks)
                {
                    if (allBook.Published.Year < 2022)
                    {
                        allBook.PublishedYear = allBook.Published.Year;
                        bookDtoList.Add(allBook);
                    }
                }

                authorDto.AllBooks = bookDtoList;
                authors.Add(authorDto);
            }

            return authors;
        }
    }


    [Benchmark]
    public List<AuthorDTO> GetAuthorsOpt()
    {
        using (AppDbContext appDbContext = new AppDbContext())
        {
            List<AuthorDTO> list =
                appDbContext.Authors
                    .Where(x => x.Country == "Serbia" && x.Age > 26)
                    .OrderByDescending(x => x.BooksCount)
                    .Select(x => new AuthorDTO
                    {
                        UserFirstName = x.User.FirstName,
                        UserLastName = x.User.LastName,
                        UserEmail = x.User.Email,
                        UserName = x.User.UserName,
                        AllBooks = x.Books
                            .Where(x=>x.Published.Year < 2022)
                            .Select(y => new BookDto
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Published = y.Published,
                         
                        }).ToList(),
                        AuthorAge = x.Age,
                        AuthorCountry = x.Country,
                        Id = x.Id
                    })
                    
                  
                    .Take(2)
                    .ToList();


            return list;
        }
    }

    [GlobalCleanup]
    public async Task CleanUp()
    {
        await _generator.GlobalCleanup(_appDbContext);
    }
}