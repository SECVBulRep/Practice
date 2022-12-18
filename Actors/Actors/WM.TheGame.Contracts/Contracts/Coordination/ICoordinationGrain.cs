using WM.TheGame.Contracts.Implementations.Coordination;

namespace WM.TheGame.Contracts.Contracts.Coordination;

public interface ICoordinationGrain : IGrainWithStringKey
{
    Task<(int,int)> GetInfo();
    Task MoveNorth(int steps);
    Task MoveWest(int steps);
    Task MoveSouth(int steps);
    Task MoveEast(int steps);
}