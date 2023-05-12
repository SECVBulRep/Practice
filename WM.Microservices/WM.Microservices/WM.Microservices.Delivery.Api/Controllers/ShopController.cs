using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WM.Microservices.Delivery.Api.Repository;

namespace WM.Microservices.Delivery.Api.Controllers;

[ApiController]
[Route("api/m/[controller]/[action]")]
public class ShopController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ShopController(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public ActionResult TestConection()
    {
        Console.WriteLine("--> Inbound Post DeliveryService");
        return Ok("inbound test");
    }
    
    

}