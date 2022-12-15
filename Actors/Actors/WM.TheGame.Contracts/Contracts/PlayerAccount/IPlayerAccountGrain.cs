namespace WM.TheGame.Contracts.Contracts.PlayerAccount
{
    public interface IPlayerAccountGrain: IGrainWithStringKey
    {
        [Transaction(TransactionOption.Join)]
        Task Withdraw(decimal amount);

        [Transaction(TransactionOption.Join)]
        Task Deposit(decimal amount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> GetBalance();
    }
}