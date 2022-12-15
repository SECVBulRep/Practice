using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;
using WM.TheGame.Contracts.Contracts.PlayerAccount;

namespace WM.TheGame.Contracts.Implementations.PlayerAccount;

[Reentrant]
public class PlayerAccountGrainGrain : Grain, IPlayerAccountGrain
{
    private readonly ITransactionalState<Balance> _balance;
    private ILogger<PlayerAccountGrainGrain> _logger;

    public PlayerAccountGrainGrain(
        [TransactionalState(nameof(balance))]
        ITransactionalState<Balance> balance, ILogger<PlayerAccountGrainGrain> logger)
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