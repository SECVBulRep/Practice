using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Runtime;
using WM.TheGame.Contracts.Contracts;
using WM.TheGame.Contracts.Contracts.Chat;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;
using WM.TheGame.WebApi.Model;

namespace WM.TheGame.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly IClusterClient _clusterClient;

    public PlayerController(ILogger<PlayerController> logger,
        IClusterClient clusterClient
    )
    {
        _logger = logger;
        _clusterClient = clusterClient;
    }

    [HttpPut]
    [Route("Connect")]
    public async Task<IActionResult> Connect(PlayerInfo playerInfo)
    {
        var game=  _clusterClient.GetGrain<IGameGrain>("WoW");
        var gamer = _clusterClient.GetGrain<IPlayerGrain>(playerInfo.PlayerName);
        
        await game.ConnectPlayer(gamer);
        await game.SendMessageToChat($"hi all from {playerInfo.PlayerName}");
        return Accepted();
    }
    
    [HttpPut]
    [Route("CheckPlayers")]
    public Task<IActionResult> Connect(List<PlayerInfo> playersInfo)
    {
        List<Task> tasks = new List<Task>();

        foreach (var playerInfo in playersInfo)
        {
            var anotherPlayers = playersInfo.Where(x => x.PlayerName != playerInfo.PlayerName).ToList();
           
            var callerGamer = _clusterClient.GetGrain<IPlayerGrain>(playerInfo.PlayerName); 
            
            foreach (var anotherPlayer in anotherPlayers)
            {
                var answererGamer = _clusterClient.GetGrain<IPlayerGrain>(anotherPlayer.PlayerName);

                var task = callerGamer.CheckPlayer(answererGamer);
                tasks.Add(task);
            }
        }
        
        Task.WaitAll(tasks.ToArray(),CancellationToken.None);
        return Task.FromResult<IActionResult>(Accepted());
    }
    
    
}