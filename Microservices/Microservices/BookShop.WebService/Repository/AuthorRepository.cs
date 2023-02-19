using BookShop.WebService.Models;
using Microservices.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace BookShop.WebService.Repository;

public class AuthorRepository: GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(DbContext context) : base(context)
    {
    }
}