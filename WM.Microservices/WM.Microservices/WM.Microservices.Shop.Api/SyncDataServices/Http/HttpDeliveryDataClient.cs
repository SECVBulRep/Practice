using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WM.Microservices.Shop.Api.Dtos;

namespace WM.Microservices.Shop.Api.SyncDataServices.Http;

public class HttpDeliveryDataClient : IDeliveryDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpDeliveryDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendShopToDelivery(ProductReadDto productReadDto)
    {
        JsonSerializerOptions options = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };
        
        var httpContent = new StringContent(
            JsonSerializer.Serialize(productReadDto,options), Encoding.UTF8, "application/json"
        );
        var response = await _httpClient.PostAsync($"{_configuration["deliverySystem"]}", httpContent);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "--> sync post to delivery service is OK !!!"
            : "--> sync post to delivery service is NOT OK !!!");
    }
}