using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BookShop.WebService.Dtos;

namespace BookShop.WebService.SyncDataServices.Http;

public class HttpManagingDataClient : IManagingDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpManagingDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendShopToManaging(AuthorReadDto authorReadDto)
    {
        JsonSerializerOptions options = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };
        
        var httpContent = new StringContent(
            JsonSerializer.Serialize(authorReadDto,options), Encoding.UTF8, "application/json"
        );
        var response = await _httpClient.PostAsync($"{_configuration["managingSystem"]}", httpContent);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "--> sync post to managing service is OK !!!"
            : "--> sync post to managing service is NOT OK !!!");
    }
}