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
    public bool CanFall = true;
    public bool CanBeMatched = true;

    public bool WantsToGoDown()
    {
        return true;
    }

}
