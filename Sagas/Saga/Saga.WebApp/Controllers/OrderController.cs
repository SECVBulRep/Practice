using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.WebApp.Infra;
using Saga.WebApp.Model;
using Saga.WebApp.RB;

namespace Saga.WebApp.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private IOrderDataAccess _dataAccess;
    private ISendEndpointProvider _sendEndpointProvider;
    private IBusControl _busControl;

    public OrderController(
        IOrderDataAccess dataAccess,
        ISendEndpointProvider sendEndpointProvider,
        IBusControl busControl)
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
        model.OrderId = Guid.NewGuid();
        _dataAccess.SaveOrder(model);
        
        
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:order-listener"));
        await sendEndpoint.Send<IOrderMessage>(new
        {
            OrderId = model.OrderId,
            ProductName = model.ProductName,
            CardNumber = model.CardNumber
        });


        return Ok(model);
    }

    [HttpPost("create-using-state-machine")]
    public async Task<ActionResult> CreateOrderUsingStateMachine([FromBody] OrderModel model)
    {
        model.OrderId = Guid.NewGuid();
        _dataAccess.SaveOrder(model);
        
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:saga-bus-queue"));
        await sendEndpoint.Send<IOrderStartEvent>(new
        {
            OrderId = model.OrderId,
            ProductName = model.ProductName,
            CardNumber = model.CardNumber
        });
        return Ok("success");
    }

    [HttpGet("get-order")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        return Ok(_dataAccess.GetOrder(orderId));
    }
    
    
    [HttpDelete("delete-order")]
    public async Task<IActionResult>  DeleteOrder(Guid orderId)
    {
        return Ok(_dataAccess.DeleteOrder(orderId));
    }
    
}