using System.Runtime.Intrinsics.X86;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WM.Microservices.Shop.Api.AsyncDataServices;
using WM.Microservices.Shop.Api.Dtos;
using WM.Microservices.Shop.Api.Models;
using WM.Microservices.Shop.Api.Repository;
using WM.Microservices.Shop.Api.SyncDataServices.Http;

namespace WM.Microservices.Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDeliveryDataClient _deliveryDataClient;
    private readonly ShopFaker _shopFaker;
    private readonly IMessageBusClient _messageBusClient;


    public ProductController(ILogger<ProductController> logger, IProductRepository productRepository, IMapper mapper,
        IDeliveryDataClient deliveryDataClient, ShopFaker shopFaker, IMessageBusClient messageBusClient
    )
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _deliveryDataClient = deliveryDataClient;
        _shopFaker = shopFaker;
        _messageBusClient = messageBusClient;
    }

    [HttpGet(Name = "Get")]
    public ActionResult<ProductReadDto> Get(int id)
    {
        var product = _productRepository.Get(x => x.Id == id).SingleOrDefault();

        if (product == null)
            return NotFound();
        var ret = _mapper.Map<ProductReadDto>(product);
        return Ok(ret);
    }

    [HttpGet(Name = "GetAll")]
    public ActionResult<IEnumerable<ProductReadDto>> GetAll()
    {
        var all = _productRepository.Get().ToList();
        var ret = _mapper.Map<List<ProductReadDto>>(all);
        return Ok(ret);
    }

    [HttpPut(Name = "Create")]
    public async Task<ActionResult<ProductReadDto>> Create(ProductCreateDto productCreateDto)
    {
        var product = _shopFaker.GetProductGenerator().Generate();
        _productRepository.Create(product);
        var ret = _mapper.Map<ProductReadDto>(product);
        try
        {
            await _deliveryDataClient.SendShopToDelivery(ret);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            var message = _mapper.Map<ProductPublishedDto>(ret);
            message.Event = "Product_Published";
            _messageBusClient.PublishNewProduct(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }
        
        
        return CreatedAtRoute(nameof(Get), new {Id = product.Id}, product);
    }
}