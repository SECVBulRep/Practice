using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdServ.IdentityServer.Models;

namespace IdServ.IdentityServer.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public AuthController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        return View();
    }
 
}

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; }
    
    [Required]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
}