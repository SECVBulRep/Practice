using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;
using WM.TheGame.Contracts.Contracts.Chat;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;

namespace WM.TheGame.Contracts.Implementations.Game;

[StorageProvider(ProviderName = "Wm.GrainStorage")]
public class GameGrain : Grain<GameState>, IGameGrain
{
    private readonly ILogger<GameGrain> _logger;
    
    private readonly ObserverManager<IChat> _subsManager;
    public GameGrain(ILogger<GameGrain> logger)
    {
        _logger = logger;
        
        _subsManager =
            new ObserverManager<IChat>(
                TimeSpan.FromMinutes(5), logger, "subs");
    }
 
    public Task Subscribe(IChat observer)
    {
        _subsManager.Subscribe(observer, observer);
        return Task.CompletedTask;
    }

    public Task UnSubscribe(IChat observer)
    {
        _subsManager.Unsubscribe(observer);
        return Task.CompletedTask;
    }

    public Task SendUpdateMessage(string message)
    {
        _subsManager.Notify(s => s.ReceiveMessage(message));
        return Task.CompletedTask;
    }
    
    [OneWay]
    public async Task StartGame()
    {
        _logger.Info($"Game status {State.GameStatus}");
        if (State.GameStatus == GameStatus.Stoped)
        {
            _logger.Info($"Game is starting ...");
            Thread.Sleep(10000);
            State.GameStatus = GameStatus.Started;
            State.Players = new List<string>();
            await this.WriteStateAsync();
            _logger.Info($"Game started");
        }
        else
        {
            _logger.Info($"Game already started");
        }
    }

    public async Task StopGame()
    {
        if (State.GameStatus == GameStatus.Started)
        {
            _logger.Info($"Game is stoping ...");
            State.GameStatus = GameStatus.Stoped;
            await WriteStateAsync();
            _logger.Info($"Game is stopped ...");
        }
        else
        { 
            _logger.Info($"Game is already stopped ...");
        }
    }

    public async Task<bool> ConnectPlayer(IPlayerGrain playerGrain)
    {
        _logger.Info($"Player {playerGrain.GetPrimaryKeyString()} is trying to connect ... ");

        if (State.GameStatus == GameStatus.Started)
        {
            State.Players!.Add(playerGrain.GetPrimaryKeyString());
            await WriteStateAsync();
            return true;
        }

        return false;
    }
    
  
    
}