using Microsoft.AspNetCore.Mvc;

namespace Managing.WebService.Controllers;

[ApiController]
[Route("api/m/[controller]/[action]")]
public class BookShopController : ControllerBase
{
    public BookShopController()
    { }

    
    [HttpPost]
    public ActionResult TestConection()
    {
        Console.WriteLine("--> Inbound Post ManagingService");
        return Ok("inbound test");
    }

}