using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdServ.ClientJava.Models;

namespace IdServ.ClientJava.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Callback()
    {
        var html = System.IO.File.ReadAllText(@"./callback.html");
        
        return base.Content(html, "text/html");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}