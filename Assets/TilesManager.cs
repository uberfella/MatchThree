using UnityEngine;

public class TilesManager : MonoBehaviour
{
    private TileType[,] grid;
    private static int rows = 3;
    private static int cols = 5;

    void Start()
    {
        grid = new TileType[rows + 1, cols];
    }

    void Update()
    {
        
    }
}
