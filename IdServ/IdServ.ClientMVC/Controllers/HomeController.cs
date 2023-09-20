using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using IdServ.ClientMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace IdServ.ClientMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IHttpClientFactory _httpClientFactory;
    
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Privacy()
    {
        var id_token = await HttpContext.GetTokenAsync("id_token");
        var access_token = await HttpContext.GetTokenAsync("access_token");
      
       
        var handler = new JwtSecurityTokenHandler();
        
        var userInfo = new AuthInfo();
        userInfo.IdToken =  handler.ReadJwtToken(id_token);
        userInfo.AccessToken = handler.ReadJwtToken(access_token);
        userInfo.UserInfo = User.Claims;

        var ordersClient = _httpClientFactory.CreateClient();

        var result = "noinfo";
        try
        {
            ordersClient.SetBearerToken(access_token!);
            result = await ordersClient.GetStringAsync($"https://localhost:5072/Site/GetSecrets");
        }
        catch (Exception e)
        {
        }

        ViewBag.Message = result;
        
        return View(userInfo);
    }

    [Authorize(Policy = "HasDateOfBirth")]
    public async Task<IActionResult> Privacy2()
    {
        var id_token = await HttpContext.GetTokenAsync("id_token");
        var access_token = await HttpContext.GetTokenAsync("access_token");
      
       
        var handler = new JwtSecurityTokenHandler();
        
        var userInfo = new AuthInfo();
        userInfo.IdToken =  handler.ReadJwtToken(id_token);
        userInfo.AccessToken = handler.ReadJwtToken(access_token);
        userInfo.UserInfo = User.Claims;

        var ordersClient = _httpClientFactory.CreateClient();

        var result = "noinfo";
        try
        {
            ordersClient.SetBearerToken(access_token!);
            result = await ordersClient.GetStringAsync($"https://localhost:5072/Site/GetSecrets");
        }
        catch (Exception e)
        {
        }

        ViewBag.Message = result;
        
        return View(userInfo);
    }
    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class AuthInfo
{
    public JwtSecurityToken IdToken { get; set; }

    public JwtSecurityToken AccessToken { get; set; }

    public IEnumerable<Claim> UserInfo { get; set; }
}