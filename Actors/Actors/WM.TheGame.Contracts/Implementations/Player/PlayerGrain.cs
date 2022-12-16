using System.Reflection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Serialization.Invocation;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;
using WM.TheGame.Contracts.Contracts.PlayerAccount;
using WM.TheGame.Contracts.Implementations.Game;

namespace WM.TheGame.Contracts.Implementations.Player;

//[Reentrant] 
[MayInterleave(nameof(ArgHasInterleaveAttribute))]
//[StorageProvider(ProviderName = "Wm.GrainStorage")]
public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
{
    private readonly ILogger<PlayerGrain> _logger;

    public PlayerGrain(ILogger<PlayerGrain> logger)
    {
        _logger = logger;
    }

    public static bool ArgHasInterleaveAttribute(IInvokable req)
    {
        return req.GetArgument(0)?.GetType().GetCustomAttribute<InterleaveAttribute>() != null;
    }

    public async Task JoinGame(IGameGrain game)
    {
        this.State.CurrentGame = game.GetPrimaryKeyString();
        await WriteStateAsync();

        _logger.LogInformation(
            $"Player {this.GetPrimaryKeyString()} joined game {game.GetPrimaryKeyString()}");
    }

    public async Task LeaveGame(IGameGrain game)
    {
        this.State.CurrentGame = null;
        await WriteStateAsync();

        _logger.LogInformation(
            $"Player {this.GetPrimaryKeyString()} left game {game.GetPrimaryKeyString()}");
    }

    public void NotificationFromGame(string message)
    {
        _logger.LogInformation(
            $"Player {this.GetPrimaryKeyString()} got notification from {this.State.CurrentGame}: ~{message}~ ");
    }


    public async Task CheckPlayer(IPlayerGrain playerGrain)
    {
        _logger.LogInformation($"{this.GetPrimaryKeyString()} starts greet {playerGrain.GetPrimaryKeyString()}...");
        await playerGrain.CheckProcessor(new CheckRequest());
        _logger.LogInformation($"{this.GetPrimaryKeyString()} ends greet {playerGrain.GetPrimaryKeyString()}...");
    }


    public async Task CheckProcessor(object data)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
    }

    public Task Transfer(
        string toId,
        decimal amount) =>
        Task.WhenAll(
            GrainFactory.GetGrain<IPlayerAccountGrain>(this.GetPrimaryKeyString()).Withdraw(amount),
            GrainFactory.GetGrain<IPlayerAccountGrain>(toId).Deposit(amount));
}