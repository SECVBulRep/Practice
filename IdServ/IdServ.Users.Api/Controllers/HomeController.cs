using System.Diagnostics;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using IdServ.Users.Api.Models;

namespace IdServ.Users.Api.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private IHttpClientFactory _clientFactory;
    
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> GetOrder()
    {
        var identityClient = _clientFactory.CreateClient();
        var discoveryDocument = await identityClient.GetDiscoveryDocumentAsync("http://localhost:5008");


        var tokenResponse = await identityClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = "client_id",
            ClientSecret = "client_secret",
            Scope = "OrdersAPI",
            
        });
        
        
        var ordersClient = _clientFactory.CreateClient();

        var result = "noinfo";
        try
        {
             ordersClient.SetBearerToken(tokenResponse.AccessToken!);
             result = await ordersClient.GetStringAsync($"http://localhost:5072/Site/GetSecrets");
        }
        catch (Exception e)
        {
        }

        ViewBag.Message = result;
        
        
        return View();
    }
}