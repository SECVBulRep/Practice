using Orleans;
using WM.TheGame.Contracts.Contracts.Chat;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Implementations.Chat;

namespace WM.TheGame.WebApi.Services;

public class GameService : IHostedService
{
    private readonly ILogger<GameService> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly Chat _chat;

    public GameService(ILogger<GameService> logger,
        IClusterClient clusterClient,
        Chat chat)
    {
        _logger = logger;
        _clusterClient = clusterClient;
        _chat = chat;
    }
 
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var game=  _clusterClient.GetGrain<IGameGrain>("WoW");
        await game.StartGame();
        var obj =  _clusterClient.CreateObjectReference<IChat>(_chat);
        await game.SubscribeToChat(obj);
        await game.SendMessageToChat("Chat is started!!!");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}