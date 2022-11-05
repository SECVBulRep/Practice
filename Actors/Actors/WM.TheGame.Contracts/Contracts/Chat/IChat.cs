using Orleans;

namespace WM.TheGame.Contracts.Contracts.Chat;

public interface IChat : IGrainObserver
{
    void ReceiveMessage(string message);
}

public class Chat : IChat
{
    public void ReceiveMessage(string message)
    {
        Console.WriteLine(message);
    }
}