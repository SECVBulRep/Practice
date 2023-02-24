using BookShop.WebService.Dtos;

namespace BookShop.WebService.SyncDataServices.Http;

public interface IManagingDataClient
{
     Task SendShopToManaging(AuthorReadDto authorReadDto);
}