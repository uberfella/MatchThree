using UnityEngine;

public enum TileType
{
    Red,
    Blue,
    Green,
    Yellow
}

public class Cell
{
    public TileType Type;

    public int X;
    public int Y;

    private TilesManager tilesManager;


    public void Init(int x, int y, TilesManager manager)
    {
        X = x;
        Y = y;
        tilesManager = manager;
    }

    public void TileGoesDownOneCell()
    {
        if (X < 0 || Y < 0)
        {
            return;
        }
        X--;
    }
}
