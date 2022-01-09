using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.WebApp.Infra;
using Saga.WebApp.Model;
using Saga.WebApp.RB;

namespace Saga.WebApp.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController :  ControllerBase
{
    private IOrderDataAccess _dataAccess;
    private ISendEndpointProvider _sendEndpointProvider;
    private IBusControl _busControl;
    
    public OrderController(
        IOrderDataAccess dataAccess, 
        ISendEndpointProvider sendEndpointProvider, IBusControl busControl)
    {
        _dataAccess = dataAccess;
        _sendEndpointProvider = sendEndpointProvider;
        _busControl = busControl;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(_dataAccess.GetAllOrder());
    }
    
    [HttpPost("create")]
    public async Task<ActionResult> Create(OrderModel model)
    {
        //var endPoint  = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:"+))

        await _busControl.Publish<IOrderMessage>(new
        {
            OrderId = model.OrderId,
            ProductName = model.ProductName,
            CardNumber= model.CardNumber
        });
        
        
        _dataAccess.SaveOrder(model);
        return Ok(model);
    }


}