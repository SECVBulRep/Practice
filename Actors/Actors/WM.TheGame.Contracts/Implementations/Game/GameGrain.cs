using Orleans;
using Orleans.Core;
using Orleans.Providers;
using Orleans.Runtime;
using WM.TheGame.Contracts.Contracts;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Implementations.Game;

namespace WM.TheGame.Contracts.Implementations;

[StorageProvider(ProviderName = "Wm.GrainStorage")]
public class GameGrain : Grain<GameState>, IGameGrain
{
    public async Task StartGame()
    {
        Console.WriteLine($"Game status {State.GameStatus}");
        if (State.GameStatus == GameStatus.Stoped)
        {
            State.GameStatus = GameStatus.Started;
            await this.WriteStateAsync();
            Console.WriteLine($"Game started");
        }
    }

    public async Task StopGame()
    {
        if (State.GameStatus == GameStatus.Started)
        {
            State.GameStatus = GameStatus.Stoped;
            await WriteStateAsync();
        }
    }
}