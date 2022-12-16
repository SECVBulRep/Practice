using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;
using WM.TheGame.Contracts.Contracts.PlayerAccount;

namespace WM.TheGame.Contracts.Implementations.PlayerAccount;

[Reentrant]
public class PlayerAccountGrain : Grain, IPlayerAccountGrain
{
    private readonly ITransactionalState<Balance> _balance;
    private ILogger<PlayerAccountGrain> _logger;

    public PlayerAccountGrain(
        [TransactionalState(nameof(balance))] ITransactionalState<Balance> balance, ILogger<PlayerAccountGrain> logger)
    {
        _balance = balance ?? throw new ArgumentNullException(nameof(balance));
        _logger = logger;
    }

    public Task Deposit(decimal amount)
    {
        _logger.LogInformation($"Deposit {amount} credits to account ");

        if (amount > 1000)
            throw new InvalidOperationException();

        return _balance.PerformUpdate(
            balance => balance.Value += amount);
    }

    public Task Withdraw(decimal amount)
    {
        return _balance.PerformUpdate(balance =>
        {
            _logger.LogInformation($"Withdrawing {amount} credits from account ");

            if (balance.Value < amount)
            {
                throw new InvalidOperationException(
                    $"Withdrawing {amount} credits from account " +
                    $"\"{this.GetPrimaryKeyString()}\" would overdraw it." +
                    $" This account has {balance.Value} credits.");
            }

            balance.Value -= amount;
        });
    }

    public Task<decimal> GetBalance()
    {
        return _balance.PerformRead(balance => balance.Value);
    }
}