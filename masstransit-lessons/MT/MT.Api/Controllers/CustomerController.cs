using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MT.SampleContracts;

namespace MT.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : Controller
{
    private readonly ILogger<CustomerController> _logger;
    private IPublishEndpoint _publishEndpoint;

    public CustomerController(ILogger<CustomerController> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id,string customerNumber)
    {
        await _publishEndpoint.Publish<ICustomerAccountClosed>(new
        {
            CustomerId = id,
            CustomerNumber = customerNumber,
            Stamp = DateTime.Now
        });
        return Ok();
    }
}