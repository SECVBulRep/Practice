using Orleans;
using Orleans.Concurrency;
using WM.TheGame.Contracts.Contracts;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;

namespace WM.TheGame.Contracts.Implementations;

public class PlayerGrain : Grain, IPlayerGrain
{
    private IGameGrain _currentGame;
    public Task<IGameGrain> GetCurrentGame()
    {
        return Task.FromResult(_currentGame);
    }

    public Task JoinGame(IGameGrain game)
    {
        _currentGame = game;

        Console.WriteLine(
            $"Player {IdentityString} joined game {game.GetPrimaryKey()}");

        return Task.CompletedTask;
    }

    [OneWay]
    public Task LeaveGame(IGameGrain game)
    {
        _currentGame = null;
        Console.WriteLine(
            $"Player {IdentityString} left game {game.GetPrimaryKey()}");

        return Task.CompletedTask;
    }
}