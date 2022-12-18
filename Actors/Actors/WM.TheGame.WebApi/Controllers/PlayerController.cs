using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Runtime;
using WM.TheGame.Contracts.Contracts;
using WM.TheGame.Contracts.Contracts.Chat;
using WM.TheGame.Contracts.Contracts.Coordination;
using WM.TheGame.Contracts.Contracts.Game;
using WM.TheGame.Contracts.Contracts.Player;
using WM.TheGame.Contracts.Contracts.PlayerAccount;
using WM.TheGame.WebApi.Model;

namespace WM.TheGame.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly ITransactionClient _transactionClient;

    public PlayerController(ILogger<PlayerController> logger,
        IClusterClient clusterClient, ITransactionClient transactionClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;
        _transactionClient = transactionClient;
    }

    [HttpPut]
    [Route("Connect")]
    public async Task<IActionResult> Connect(PlayerInfo playerInfo)
    {
        var game = _clusterClient.GetGrain<IGameGrain>("WoW");
        var gamer = _clusterClient.GetGrain<IPlayerGrain>(playerInfo.PlayerName);

        await game.ConnectPlayer(gamer);
        await game.SendMessageToChat($"hi all from {playerInfo.PlayerName}");
        return Accepted();
    }

    [HttpPut]
    [Route("CheckPlayers")]
    public Task<IActionResult> Connect(List<PlayerInfo> playersInfo)
    {
        List<Task> tasks = new List<Task>();

        foreach (var playerInfo in playersInfo)
        {
            var anotherPlayers = playersInfo.Where(x => x.PlayerName != playerInfo.PlayerName).ToList();

            var callerGamer = _clusterClient.GetGrain<IPlayerGrain>(playerInfo.PlayerName);

            foreach (var anotherPlayer in anotherPlayers)
            {
                var answererGamer = _clusterClient.GetGrain<IPlayerGrain>(anotherPlayer.PlayerName);

                var task = callerGamer.CheckPlayer(answererGamer);
                tasks.Add(task);
            }
        }

        Task.WaitAll(tasks.ToArray(), CancellationToken.None);
        return Task.FromResult<IActionResult>(Accepted());
    }


    [HttpPut]
    [Route("DebitAccount")]
    public async Task<IActionResult> DebitAccount(DebitAccountRequest playerInfo)
    {
        var fromAccount = _clusterClient.GetGrain<IPlayerAccountGrain>(playerInfo.From);
        var toAccount = _clusterClient.GetGrain<IPlayerAccountGrain>(playerInfo.To);


        try
        {
            await _transactionClient.RunTransaction(
                TransactionOption.Create,
                async () =>
                {
                    await fromAccount.Withdraw(playerInfo.Amount);
                    await toAccount.Deposit(playerInfo.Amount);
                });
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        decimal fromBalance = await fromAccount.GetBalance();
        var toBalance = await toAccount.GetBalance();

        var response = new DebitAccountResponse(fromBalance, toBalance);

        return Accepted(response);
    }


    [HttpPut]
    [Route("SendMoneyToPlayer")]
    public async Task<IActionResult> DebitAccount(SendMoneyRequest sendMoneyRequest)
    {
        IPlayerGrain sender = _clusterClient.GetGrain<IPlayerGrain>(sendMoneyRequest.From);


        try
        {
            await sender.Transfer(sendMoneyRequest.To, sendMoneyRequest.Amount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        var fromAccount = _clusterClient.GetGrain<IPlayerAccountGrain>(sendMoneyRequest.From);
        var toAccount = _clusterClient.GetGrain<IPlayerAccountGrain>(sendMoneyRequest.To);

        var response = new DebitAccountResponse(await fromAccount.GetBalance(), await toAccount.GetBalance());

        return Accepted(response);
    }


    [HttpPut]
    [Route("MovePlayer")]
    public async Task<IActionResult> MovePlayer(MoveRequest moveRequest)
    {
        var coordinationGrain = _clusterClient.GetGrain<ICoordinationGrain>(moveRequest.Player);

        switch (moveRequest.Direction)
        {
            case Direction.East:
                await coordinationGrain.MoveEast(moveRequest.Steps);
                break;
            case Direction.North:
                await coordinationGrain.MoveNorth(moveRequest.Steps);
                break;
            case Direction.South:
                await coordinationGrain.MoveSouth(moveRequest.Steps);
                break;
            case Direction.West:
                await coordinationGrain.MoveWest(moveRequest.Steps);
                break;
        }
        
        var coord = await coordinationGrain.GetInfo();
        var response = new MoveResponse(coord.Item1, coord.Item2);
        
        return Ok(response);
    }




}