using Orleans;

namespace WM.TheGame.Grains.Contracts;

public interface IGameGrain : IGrainWithGuidKey
{
    Task<string> GetName();
    
    Task SetName(string name);
}