using WM.TheGame.Grains.Contracts;

namespace WM.TheGame.Grains.Implementations;

public class GameGrain : IGameGrain
{
    private string _name; 
    
    public async Task<string> GetName()
    {
        return _name;
    }

    public Task SetName(string name)
    {
        _name = name;
        return Task.CompletedTask;
    }
}