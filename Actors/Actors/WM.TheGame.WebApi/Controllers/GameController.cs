using Microsoft.AspNetCore.Mvc;
using Orleans;
using WM.TheGame.Contracts.Contracts;
using WM.TheGame.Contracts.Contracts.Game;

namespace WM.TheGame.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly IClusterClient _clusterClient;

    public GameController(ILogger<GameController> logger,
        IClusterClient clusterClient
    )
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }
    
    
    [HttpPut]
    [Route("Start")]
    public async Task<IActionResult> Post()
    {
        var game=  _clusterClient.GetGrain<IGameGrain>("WoW");
        await game.StartGame();
        
        return Accepted();
    }

}