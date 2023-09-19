using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdServ.Orders.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdServ.Orders.Api.Controllers;

public class SiteController : Controller
{
    private readonly ILogger<SiteController> _logger;

    public SiteController(ILogger<SiteController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


   [Authorize]
    public string GetSecrets()
    {
        return "!!!!!!!!!! secretes from  orders !!!!!!!!!!!!!!";
    }

    
}