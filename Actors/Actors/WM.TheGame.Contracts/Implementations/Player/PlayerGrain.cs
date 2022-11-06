using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;
using WM.TheGame.Contracts.Implementations.Game;

namespace WM.TheGame.Contracts.Implementations.Player;

[StorageProvider(ProviderName = "Wm.GrainStorage")]
public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
{
    public async Task JoinGame(IGameGrain game)
    {
        this.State.CurrentGame = game.GetPrimaryKeyString();
        await WriteStateAsync();
        
        Console.WriteLine(
            $"Player {this.GetPrimaryKeyString()} joined game {game.GetPrimaryKeyString()}");
    }

    public async Task LeaveGame(IGameGrain game)
    {
        
        this.State.CurrentGame = null;
        await WriteStateAsync();
        
        Console.WriteLine(
            $"Player {this.GetPrimaryKeyString()} left game {game.GetPrimaryKeyString()}");
      
    }

    public void NotificationFromGame(string message)
    {
        Console.WriteLine(
            $"Player {this.GetPrimaryKeyString()} got notification from {this.State.CurrentGame}: ~{message}~ ");
    }
}