namespace API.Models;

public readonly struct SquareChimp
{
    public int Number { get; }
    public bool Revealed { get; }
    public int X { get; }
    public int Y { get; }

    public SquareChimp(int number, bool revealed, int x, int y)
    {
        Number = number;
        Revealed = revealed;
        X = x;
        Y = y;
    }
}
