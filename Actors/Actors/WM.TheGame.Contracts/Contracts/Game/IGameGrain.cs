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
        Task SubscribeToChat(IChat observer);
        Task UnSubscribeFromChat(IChat observer);
        Task SendMessageToChat(string message);
    }
}