using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private Cell[,] allTiles;

    void Start()
    {
        allTiles = new Cell[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject cellObj = Instantiate(tilePrefab, tempPosition, Quaternion.identity, transform);
                cellObj.name = "(" + i + ", " + j + ")";

                // Ensure there's a Cell component
                Cell cellComp = cellObj.GetComponent<Cell>();
                if (cellComp == null)
                {
                    cellComp = cellObj.AddComponent<Cell>();
                }

                // Set coordinates so the cell can notify the board directly
                cellComp.x = i;
                cellComp.y = j;

                // Ensure a 2D collider exists for clicks (use BoxCollider2D for a 2D match-3)
                if (cellObj.GetComponent<Collider2D>() == null)
                {
                    cellObj.AddComponent<BoxCollider2D>();
                }

                allTiles[i, j] = cellComp;
            }
        }
    }

    // Called by Cell to clear its slot before/after destruction
    public void RemoveCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        allTiles[x, y] = null;
    }

    // Fallback: remove by reference (searches grid)
    public void RemoveCell(Cell cell)
    {
        if (cell == null) return;

        if (cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height && allTiles[cell.x, cell.y] == cell)
        {
            allTiles[cell.x, cell.y] = null;
            return;
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] == cell)
                {
                    allTiles[i, j] = null;
                    return;
                }
            }
        }
    }
}
