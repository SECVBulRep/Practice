using Orleans;
using WM.TheGame.Contracts.Contracts.Game;

namespace WM.TheGame.Contracts.Contracts.Player;

public interface IPlayerGrain : IGrainWithStringKey
{
  
    Task JoinGame(IGameGrain game);
    Task LeaveGame(IGameGrain game);
}
