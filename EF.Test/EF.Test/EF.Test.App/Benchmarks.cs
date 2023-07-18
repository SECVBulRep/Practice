using System.Data;
using BenchmarkDotNet.Attributes;
using EF.Test.App.DbContext;
using EF.Test.App.Dto;
using EF.Test.App.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF.Test.App;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
[HideColumns("Job", "StdDev")]
public class Benchmarks
{
    private Random _random;
    private CompanyGenerator _generator;
    private IDbConnection _dbConnection;

    [GlobalSetup]
    public async Task Setup()
    {
        _random = new Random(420);


        //  _generator = new CompanyGenerator(_random);
        // await _generator.Generate(500, _appDbContext);
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

    // [Benchmark]
    // public List<AuthorDTO> GetAuthorsOptimized()
    // {
    //     using (AppDbContext appDbContext = new AppDbContext())
    //     {
    //         List<AuthorDTO> list =
    //             appDbContext.Authors
    //                 .Include<Author, User>(x => x.User)
    //                 .Include(x => x.Books.Where(x => x.Published.Year < 2022))
    //                 .Where(x => x.Country == "Serbia" && x.Age > 26)
    //                 .OrderByDescending(x => x.BooksCount)
    //                 .Select(x => new AuthorDTO
    //                 {
    //                     UserFirstName = x.User.FirstName,
    //                     UserLastName = x.User.LastName,
    //                     UserEmail = x.User.Email,
    //                     UserName = x.User.UserName,
    //                     AllBooks =
    //                         x.Books
    //                             .Select(y => new BookDto
    //                             {
    //                                 Id = y.Id,
    //                                 Name = y.Name,
    //                                 Published = y.Published
    //                             }).ToList(),
    //                     AuthorAge = x.Age,
    //                     AuthorCountry = x.Country,
    //                     Id = x.Id
    //                 })
    //                 .Take(2).ToList();
    //         return list;
    //     }
    // }


    private static readonly Func<AppDbContext, IAsyncEnumerable<Author>> GetAuthorsOptAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (AppDbContext context) =>
                context.Authors
                    .Include<Author, User>(x => x.User)
                    .Include(x => x.Books.Where(x => x.Published.Year < 2022))
                    .Where(x => x.Country == "Serbia" && x.Age > 26)
                    .OrderByDescending(x => x.BooksCount).Take(2)
        );

    [Benchmark]
    public async Task<List<AuthorDTO>> GetAuthorsOptimizedCompiled()
    {
        using (AppDbContext appDbContext = new AppDbContext())
        {
            var list = GetAuthorsOptAsync(appDbContext);

            List<AuthorDTO> ret = new List<AuthorDTO>();

            await foreach (var x in list)
            {
                var item = new AuthorDTO
                {
                    UserFirstName = x.User.FirstName,
                    UserLastName = x.User.LastName,
                    UserEmail = x.User.Email,
                    UserName = x.User.UserName,
                    AllBooks = x.Books
                        .Select(y => new BookDto
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Published = y.Published,
                        }).ToList(),
                    AuthorAge = x.Age,
                    AuthorCountry = x.Country,
                    Id = x.Id
                };
                ret.Add(item);
            }
            return ret;
        }
    }


    [GlobalCleanup]
        public async Task CleanUp()
        {
            // await _generator.GlobalCleanup(_appDbContext);
        }
    }