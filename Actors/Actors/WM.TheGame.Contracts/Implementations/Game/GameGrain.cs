﻿using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime;
using WM.TheGame.Contracts.Contracts.Chat;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;

namespace WM.TheGame.Contracts.Implementations.Game;

//[StorageProvider(ProviderName = "Wm.GrainStorage")]
public class GameGrain : Grain<GameState>, IGameGrain
{
   // private IDisposable? timer;

    private readonly ILogger<GameGrain> _logger;

    private readonly ObserverManager<IChat> _subsManager;

    private readonly ObserverManager<IPlayerGrain> _playersManager;

    public GameGrain(ILogger<GameGrain> logger)
    {
        _logger = logger;

        _subsManager =
            new ObserverManager<IChat>(
                TimeSpan.FromMinutes(5), logger, "subs");

        _playersManager =
            new ObserverManager<IPlayerGrain>(
                TimeSpan.FromMinutes(5), logger, "players");
    }


    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
        /*  timer = RegisterTimer(state =>
        {
            Console.WriteLine($"Health check {DateTime.Now}");
            return Task.CompletedTask;
        }, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1));
        await this.RegisterOrUpdateReminder("GameReminder", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60));
        await base.OnActivateAsync(cancellationToken);*/
    }


    public Task SubscribeToChat(IChat observer)
    {
        _subsManager.Subscribe(observer, observer);
        return Task.CompletedTask;
    }

    public Task UnSubscribeFromChat(IChat observer)
    {
        _subsManager.Unsubscribe(observer);
        return Task.CompletedTask;
    }

    public Task SendMessageToChat(string message)
    {
        _subsManager.Notify(s => s.ReceiveMessage(message));
        return Task.CompletedTask;
    }

    [OneWay]
    public async Task StartGame()
    {
        Thread.Sleep(3000);
        _logger.LogInformation($"Game status {State.GameStatus}");
        if (State.GameStatus == GameStatus.Stoped)
        {
            _logger.LogInformation($"Game is starting ...");
            Thread.Sleep(10000);
            State.GameStatus = GameStatus.Started;
            State.Players = new List<string>();
            await this.WriteStateAsync();
            _logger.LogInformation($"Game started");
        }
        else
        {
            _logger.LogInformation($"Game already started");
        }
    }

    public async Task StopGame()
    {
        if (State.GameStatus == GameStatus.Started)
        {
            _logger.LogInformation($"Game is stoping ...");
            State.GameStatus = GameStatus.Stoped;
            await WriteStateAsync();
            _playersManager.Notify(s => s.NotificationFromGame("Game is stopping ..."));
            _logger.LogInformation($"Game is stopped ...");
        }
        else
        {
            _logger.LogInformation($"Game is already stopped ...");
        }
    }

    public async Task<bool> ConnectPlayer(IPlayerGrain playerGrain)
    {
        _logger.LogInformation($"Player {playerGrain.GetPrimaryKeyString()} is trying to connect ... ");

        if (State.GameStatus == GameStatus.Started)
        {
            State.Players!.Add(playerGrain.GetPrimaryKeyString());
            _playersManager.Subscribe(playerGrain, playerGrain);
            await playerGrain.JoinGame(this);
            await WriteStateAsync();
            return true;
        }

        return false;
    }


    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        Console.WriteLine("Thanks for reminding me-- I almost forgot!");
        return Task.CompletedTask;
    }
}