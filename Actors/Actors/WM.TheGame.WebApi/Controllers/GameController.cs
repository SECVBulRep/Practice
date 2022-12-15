using Microsoft.AspNetCore.Mvc;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Implementations.Chat;

namespace WM.TheGame.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly Chat _chat;

    public GameController(ILogger<GameController> logger,
        IClusterClient clusterClient,
        Chat chat
    )
    {
        _logger = logger;
        _clusterClient = clusterClient;
        _chat = chat;
    }

    [HttpPut]
    [Route("Start")]
    public async Task<IActionResult> Start()
    {
        var game = _clusterClient.GetGrain<IGameGrain>("WoW");
        await game.StartGame();
        return Accepted();
    }

    [HttpPut]
    [Route("Stop")]
    public async Task<IActionResult> Stop()
    {
        var game = _clusterClient.GetGrain<IGameGrain>("WoW");
        await game.StopGame();
        return Accepted();
    }
}