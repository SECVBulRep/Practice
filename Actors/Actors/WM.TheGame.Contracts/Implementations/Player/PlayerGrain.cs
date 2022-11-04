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
        Console.WriteLine(
            $"Player {IdentityString} joined game {game.GetPrimaryKey()}");

        if (await game.ConnectPlayer(this))
        {
            State.CurrentGame = game.GetPrimaryKeyString();
        }
    }

    public Task LeaveGame(IGameGrain game)
    {
        Console.WriteLine(
            $"Player {IdentityString} left game {game.GetPrimaryKey()}");
        return Task.CompletedTask;
    }
}