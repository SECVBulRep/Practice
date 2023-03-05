using Microsoft.AspNetCore.Mvc;

namespace WM.Microservices.Delivery.Api.Controllers;

[ApiController]
[Route("api/m/[controller]/[action]")]
public class ShopController : ControllerBase
{
    public ShopController()
    { }

    
    [HttpPost]
    public ActionResult TestConection()
    {
        Console.WriteLine("--> Inbound Post DeliveryService");
        return Ok("inbound test");
    }

}