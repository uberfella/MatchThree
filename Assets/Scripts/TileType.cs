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
    public bool CanFall = false;
    public bool CanBeMatched = false;

    public bool WantsToGoDown()
    {
        return true;
    }

}
