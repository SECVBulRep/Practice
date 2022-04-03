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


    public OrderController(
        ILogger<OrderController> logger,
        IRequestClient<ISubmitOrder> requestClient)
    {
        _logger = logger;
        _requestClient = requestClient;
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
}