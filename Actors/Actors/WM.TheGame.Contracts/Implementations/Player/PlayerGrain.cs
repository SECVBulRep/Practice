using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Providers;
using Orleans.Runtime;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;
using WM.TheGame.Contracts.Implementations.Game;

namespace WM.TheGame.Contracts.Implementations.Player;

[StorageProvider(ProviderName = "Wm.GrainStorage")]
public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
{
    private readonly ILogger<PlayerGrain> _logger;

    public PlayerGrain(ILogger<PlayerGrain> logger)
    {
        _logger = logger;
    }
    
    public async Task JoinGame(IGameGrain game)
    {
        this.State.CurrentGame = game.GetPrimaryKeyString();
        await WriteStateAsync();
        
        _logger.Info(
            $"Player {this.GetPrimaryKeyString()} joined game {game.GetPrimaryKeyString()}");
    }

    public async Task LeaveGame(IGameGrain game)
    {
        
        this.State.CurrentGame = null;
        await WriteStateAsync();
        
        _logger.Info(
            $"Player {this.GetPrimaryKeyString()} left game {game.GetPrimaryKeyString()}");
      
    }

    public void NotificationFromGame(string message)
    {
        _logger.Info(
            $"Player {this.GetPrimaryKeyString()} got notification from {this.State.CurrentGame}: ~{message}~ ");
    }

    public async Task CheckPlayer(IPlayerGrain playerGrain)
    {
        _logger.Info($"{this.GetPrimaryKeyString()} starts greet {playerGrain.GetPrimaryKeyString()}...");
        await playerGrain.CheckProcessor();
        _logger.Info($"{this.GetPrimaryKeyString()} ends greet {playerGrain.GetPrimaryKeyString()}...");
    }

    public async Task CheckProcessor()
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
    }
}