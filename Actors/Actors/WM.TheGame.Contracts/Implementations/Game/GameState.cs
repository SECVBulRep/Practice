namespace WM.TheGame.Contracts.Implementations.Game;

public class GameState
{
    public GameStatus GameStatus { get; set; }

    public List<string>? Players { get; set; }
}

