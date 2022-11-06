using Orleans;

namespace WM.TheGame.Contracts.Contracts.Chat;

public interface IChat : IGrainObserver
{
    void ReceiveMessage(string message);
}