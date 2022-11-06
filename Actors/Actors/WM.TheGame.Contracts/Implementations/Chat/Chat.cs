using Microsoft.Extensions.Logging;
using WM.TheGame.Contracts.Contracts.Chat;

namespace WM.TheGame.Contracts.Implementations.Chat;

public class Chat : IChat
{
    public Chat()
    { }
    public void ReceiveMessage(string message)
    {
        Console.WriteLine(message);
    }
}