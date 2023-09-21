using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using IdServ.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IdServ.IdentityServer.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    private IIdentityServerInteractionService _identityServerInteractionService;
    public AuthController(ILogger<HomeController> logger,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, IIdentityServerInteractionService identityServerInteractionService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _identityServerInteractionService = identityServerInteractionService;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null)
        {
            ModelState.AddModelError("username","not found");
            return View(model);
        }

        var signResult = await _signInManager.PasswordSignInAsync(user, model.Password,false,false);

        if (signResult.Succeeded)
            return Redirect(model.ReturnUrl);
        
        return View();
    }


    public async Task<IActionResult> Logout(string logoutid)
    {
        await _signInManager.SignOutAsync();
        var logoutResult = await _identityServerInteractionService.GetLogoutContextAsync(logoutid);
        return Redirect(logoutResult.PostLogoutRedirectUri);
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