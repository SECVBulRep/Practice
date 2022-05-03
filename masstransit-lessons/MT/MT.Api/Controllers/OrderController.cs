using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MT.SampleContracts;

namespace MT.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private IRequestClient<ISubmitOrder> _requestClient;
    private ISendEndpointProvider _sendEndpointProvider;
    private IRequestClient<ICheckOrder> _checkOrderRequestClient;



    public OrderController(
        ILogger<OrderController> logger,
        IRequestClient<ISubmitOrder> requestClient, 
        ISendEndpointProvider sendEndpointProvider,
        IRequestClient<ICheckOrder> checkOrderRequestClient)
    {
        _logger = logger;
        _requestClient = requestClient;
        _sendEndpointProvider = sendEndpointProvider;
        _checkOrderRequestClient = checkOrderRequestClient;
    }



    [HttpGet]
    public async Task<ActionResult> Get(Guid id)
    {
        //перезапусти райдер если тупит
        var (status, notFound) = await _checkOrderRequestClient.GetResponse<IOrderStatus, IOrderNotFound>(new 
        {
            OrderId = id
        });

        var tasks = new List<Task>
        {
            status,
            notFound
        };

        int index = Task.WaitAny(tasks.ToArray());
        if (index == 0)
            return Ok(status.Result);
        return NotFound(notFound.Result);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid id, string customerId)
    {
        var (accepted, rejected) = await _requestClient.GetResponse<IOrderSubmissionAccepted, IOrderSubmissionRejected>(
            new
            {
                OrderId = id,
                TimeStamp = DateTime.Now,
                CustomerNumber = customerId
            });


        if (accepted.IsCompletedSuccessfully)
        {
            return Ok((await accepted).Message);
        }
        else
        {
            return BadRequest((await rejected).Message);
        }
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
    
}