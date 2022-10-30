using Orleans;

namespace WM.TheGame.Contracts.Contracts.Game
{
    public interface IGameGrain : IGrainWithStringKey
    {
    

        Task StartGame();
        Task StopGame();
   
    }
}