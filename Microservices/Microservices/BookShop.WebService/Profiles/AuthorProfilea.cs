using AutoMapper;
using BookShop.WebService.Dtos;
using BookShop.WebService.Models;

namespace BookShop.WebService.Profiles;

public class AuthorProfiles: Profile
{
    public AuthorProfiles()
    {
        CreateMap<Book, BookReadDto>();
        CreateMap<Author, AuthorReadDto>();
    }
}