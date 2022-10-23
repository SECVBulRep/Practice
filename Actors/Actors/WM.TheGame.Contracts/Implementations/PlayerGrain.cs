using Orleans;
using WM.TheGame.Contracts.Contracts;

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
            $"Player {GetPrimaryKey()} joined game {game.GetPrimaryKey()}");

        return Task.CompletedTask;
    }

    private string GetPrimaryKey()
    {
        throw new NotImplementedException();
    }

   
    public Task LeaveGame(IGameGrain game)
    {
        _currentGame = null;
        Console.WriteLine(
            $"Player {GetPrimaryKey()} left game {game.GetPrimaryKey()}");

        return Task.CompletedTask;
    }
}