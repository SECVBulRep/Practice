using AutoMapper;
using WM.Microservices.Shop.Api.Dtos;
using WM.Microservices.Shop.Api.Models;

namespace WM.Microservices.Shop.Api.Profiles;

public class ProductProfiles: Profile
{
    public ProductProfiles()
    {
        CreateMap<Product, ProductReadDto>();
    }
}