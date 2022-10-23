using Orleans;

namespace WM.TheGame.Contracts.Contracts;

public interface IPlayerGrain : IGrainWithGuidKey
{
    Task<IGameGrain> GetCurrentGame();
    Task JoinGame(IGameGrain game);
    Task LeaveGame(IGameGrain game);
}
