using AutoMapper;
using WM.Microservices.Delivery.Api.Dtos;
using WM.Microservices.Delivery.Api.Models;

namespace WM.Microservices.Delivery.Api.Profiles;

public class DeliveryProfiles : Profile
{
    public DeliveryProfiles()
    {
        CreateMap<Product, ProductReadDto>();
        CreateMap<Order, OrderReadDto>();
    }
}