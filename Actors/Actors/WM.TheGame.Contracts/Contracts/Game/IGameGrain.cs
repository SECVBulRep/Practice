using Orleans;
using WM.TheGame.Contracts.Contracts.Chat;
using WM.TheGame.Contracts.Contracts.Player;

namespace WM.TheGame.Contracts.Contracts.Game
{
    public interface IGameGrain : IGrainWithStringKey
    {
        Task StartGame();
        Task StopGame();
        Task<bool> ConnectPlayer(IPlayerGrain playerGrain);
        Task Subscribe(IChat observer);
        Task UnSubscribe(IChat observer);
        Task SendUpdateMessage(string message);
    }
}