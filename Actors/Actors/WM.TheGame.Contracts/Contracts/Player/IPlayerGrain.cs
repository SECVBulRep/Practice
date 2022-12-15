using Orleans.Concurrency;
using WM.TheGame.Contracts.Contracts.Game;

namespace WM.TheGame.Contracts.Contracts.Player;

public interface IPlayerGrain : IGrainWithStringKey,IGrainObserver
{
    Task JoinGame(IGameGrain game);
    Task LeaveGame(IGameGrain game);
    void NotificationFromGame(string message);
    Task CheckPlayer(IPlayerGrain playerGrain);

    [AlwaysInterleave]
    Task CheckProcessor(object data);
}
