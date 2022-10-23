using Orleans;
using WM.TheGame.Contracts.Contracts;

namespace WM.TheGame.Contracts.Implementations;

public class GameGrain :  Grain, IGameGrain
{
    private string _name; 
    
    public async Task<string> GetName()
    {
        return _name;
    }

    public Task SetName(string name)
    {
        Console.WriteLine($"Name {name} was set");
        _name = name;
        return Task.CompletedTask;
    }
}