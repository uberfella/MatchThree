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

                allTiles[i, j] = cellComp;
            }
        }
    }

    // Fallback: remove by reference (searches grid)
    public void RemoveCell(Cell cell)
    {
        if (cell == null) return;

        if (cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height && allTiles[cell.x, cell.y] == cell)
        {
            Debug.Log("");
            Debug.Log("making allTiles element null at "+cell.x+" "+ cell.y+"");
            Debug.Log("");
            allTiles[cell.x, cell.y] = null;
            return;
        }
    }

}
