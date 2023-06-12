using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WM.Microservices.Delivery.Api.Dtos;
using WM.Microservices.Delivery.Api.Models;
using WM.Microservices.Delivery.Api.Repository;

namespace WM.Microservices.Delivery.Api.Controllers;

[ApiController]
[Route("api/m/[controller]/[action]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public OrderController(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }


    [HttpGet]
    public ActionResult<IEnumerable<OrderReadDto>> GetOrders()
    {
        Console.WriteLine("--> GetOrders ");
        var result = _orderRepository.Get();
        return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(result));
    }
    
    
    [HttpPost]
    public ActionResult<IEnumerable<OrderReadDto>> AddProductToOrder(int orderId, int productId)
    {
        var order = _orderRepository.FindById(orderId);
        var product = _productRepository.FindById(productId);

        if (order == null || product == null)
            return NotFound();
        
        order.ProductsInOrder.Add(new ProductInOrder{OrderId = orderId,ProductId = productId});
        _orderRepository.Commit();

        return CreatedAtRoute(nameof(ProductController.GetProduct), new {orderid = orderId, productId = productId});
    }
    

}