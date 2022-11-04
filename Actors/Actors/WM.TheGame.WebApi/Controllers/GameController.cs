using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Runtime;
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
    public async Task<IActionResult> Start()
    {
        var game=  _clusterClient.GetGrain<IGameGrain>("WoW");
        game.InvokeOneWay(Handler);
        return Accepted();
    }

    private Task Handler(IGameGrain arg)
    {
       return arg.StartGame();
    }

    [HttpPut]
    [Route("Stop")]
    public async Task<IActionResult> Stop()
    {
        var game=  _clusterClient.GetGrain<IGameGrain>("WoW");
        await game.StopGame();
        
        return Accepted();
    }
    
    
}