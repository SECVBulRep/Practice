using Orleans;

namespace WM.TheGame.Contracts.Contracts;

public interface IGameGrain : IGrainWithGuidKey
{
    Task<string> GetName();
    
    Task SetName(string name);
}