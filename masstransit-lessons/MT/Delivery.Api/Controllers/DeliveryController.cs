using Delivery.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class DeliveryController :
    ControllerBase
{
    readonly ILogger<DeliveryController> _logger;
    readonly IPublishEndpoint _publishEndpoint;

    public DeliveryController(ILogger<DeliveryController> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Get(Guid id)
    {
        _logger.LogInformation($"Time to delivery the order: {id}");

        await _publishEndpoint.Publish<IOrderDeliveryRequesting>(new
        {
            OrderId = id
        });

        return Accepted();
    }
}