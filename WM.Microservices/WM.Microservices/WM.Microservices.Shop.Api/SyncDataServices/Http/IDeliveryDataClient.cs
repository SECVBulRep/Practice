using WM.Microservices.Shop.Api.Dtos;

namespace WM.Microservices.Shop.Api.SyncDataServices.Http;

public interface IDeliveryDataClient
{
     Task SendShopToDelivery(ProductReadDto productReadDto);
}