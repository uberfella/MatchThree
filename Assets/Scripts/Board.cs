using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private Cell[,] allTiles;

    private int readIndex = 0;
    private int writeIndex = 0;

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
                GameObject cellObject = Instantiate(tilePrefab, tempPosition, Quaternion.identity, transform);
                cellObject.name = "(" + i + ", " + j + ")";

                // Ensure there's a Cell component
                Cell cellComponent = cellObject.GetComponent<Cell>();
                if (cellComponent == null)
                {
                    cellComponent = cellObject.AddComponent<Cell>();
                }

                // Set coordinates so the cell can notify the board directly
                cellComponent.x = i;
                cellComponent.y = j;

                allTiles[i, j] = cellComponent;
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

    private void ApplyGravityAlgo() 
    {
        for (int j = 0; j < height; j++)
        {
            if (allTiles[0, j] != null && j != writeIndex)
            {
                allTiles[0, j].ChangePosition(0, writeIndex);

                writeIndex++;
            }
        }
    }

    private void CheckForEmptyTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] == null)
                {
                    Debug.Log("");
                    Debug.Log($"Empty tile at ({i}, {j})");
                    Debug.Log("");

                }
            }
        }
    }

    private void CheckColumnForEmptyTiles(int columnToCheck)
    {
        for (int j = 0; j < height; j++)
        {
            if (allTiles[columnToCheck, j] == null)
            {
                Debug.Log("");
                Debug.Log($"Empty tile at ({columnToCheck}, {j})");
                Debug.Log("");
            }
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

}
