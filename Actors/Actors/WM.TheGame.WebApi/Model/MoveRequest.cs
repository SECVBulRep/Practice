namespace WM.TheGame.WebApi.Model;

public record class MoveRequest(string Player,int Steps, Direction Direction);
public record class MoveResponse(int X, int Y);

public enum Direction
{
    North,
    South, 
    West,
    East
}