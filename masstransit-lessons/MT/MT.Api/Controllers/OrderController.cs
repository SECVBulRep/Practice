using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MT.SampleContracts;
using MT.SampleContracts.DTO;

namespace MT.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private IRequestClient<ISubmitOrder> _requestClient;
    private ISendEndpointProvider _sendEndpointProvider;
    private IRequestClient<ICheckOrder> _checkOrderRequestClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderController(
        ILogger<OrderController> logger,
        IRequestClient<ISubmitOrder> requestClient, 
        ISendEndpointProvider sendEndpointProvider,
        IRequestClient<ICheckOrder> checkOrderRequestClient,
        IPublishEndpoint publishEndpoint
        
        )
    {
        _logger = logger;
        _requestClient = requestClient;
        _sendEndpointProvider = sendEndpointProvider;
        _checkOrderRequestClient = checkOrderRequestClient;
        _publishEndpoint = publishEndpoint;
    }



    [HttpGet]
    public async Task<ActionResult> Get(Guid id)
    {
        //перезапусти райдер если тупит
        var (status, notFound) = await _checkOrderRequestClient.GetResponse<IOrderStatus, IOrderNotFound>(new 
        {
            OrderId = id
        });
        
        var index = Task.WaitAny(status, notFound);
        if (index == 0)
        {
            var ret = new OrderStatusModel();
            ret.State = status.Result.Message.State;
            return Ok(ret);
        }
        return NotFound(notFound.Result);
    }

    [HttpPost]
    public async Task<IActionResult> Post(OrderModel model)
    {
        var (accepted, rejected) = await _requestClient.GetResponse<IOrderSubmissionAccepted, IOrderSubmissionRejected>(
            new
            {
                OrderId = model.Id,
                TimeStamp = DateTime.Now,
                CustomerNumber = model.CustomerId,
                PaymentCardNumber = model.PaymentCardNumber
            });
        var index = Task.WaitAny(accepted, rejected);
        if(index==0)
            return Ok((await accepted).Message);
        return BadRequest((await rejected).Message);
    }
    
    [HttpPut]
    public async Task<IActionResult> Put(Guid id, string customerId)
    {

        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:submit-order"));
        await endpoint.Send<ISubmitOrder>(new
        {
            OrderId = id,
            TimeStamp = DateTime.Now,
            CustomerNumber = customerId
        });
        return Accepted();
    }
    
    
    [HttpPatch]
    public async Task<IActionResult> Patch(Guid id)
    {
        await _publishEndpoint.Publish<IOrderAccepted>(new
        {
            OrderId = id,
            TimeStamp = DateTime.Now,
        });
        return Accepted();
        
    }
    
}