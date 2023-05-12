using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WM.Microservices.Delivery.Api.Dtos;
using WM.Microservices.Delivery.Api.Models;
using WM.Microservices.Delivery.Api.Repository;

namespace WM.Microservices.Delivery.Api.Controllers;

[ApiController]
[Route("api/m/orders/{orderid}/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductController(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }


    [HttpGet]
    public ActionResult<IEnumerable<ProductReadDto>> GetProducts(int orderid)
    {
        Console.WriteLine("--> GetOrders ");
        Order? result = _orderRepository.FindById(orderid);

        if (result != null)
            return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(result.Products));
        else
            return NotFound();
    }

    [HttpGet("productId", Name = "getProductFromOrder")]
    public ActionResult<ProductReadDto> GetProduct(int orderid, int productId)
    {
        Console.WriteLine("--> GetOrders ");
        var result = _orderRepository.GetWithInclude(x => x.Id == orderid, i => i.Products).SingleOrDefault();

        if (result != null)
            return Ok(_mapper.Map<ProductReadDto>(result.Products.Where(x=>x.Id==productId)));
        else
            return NotFound();
    }

}