using WM.Microservices.Shop.Api.Dtos;

namespace WM.Microservices.Shop.Api.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishNewProduct(ProductPublishedDto productPublishedDto);
}