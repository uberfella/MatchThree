using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class BlockGravityDrop : MonoBehaviour
{
    public GameObject tileBlue;
    private GridManager gridManager;
    private GameObject[,] grid;

    void Start()
    {

        gridManager = Object.FindFirstObjectByType<GridManager>();
        //cols = gridManager.cols;
        //rows = gridManager.rows;

        // Access the grid
        if (gridManager != null && gridManager.grid != null)
        {
            // Example: Access a specific GameObject in the grid
            GameObject obj = gridManager.grid[0, 0];
            if (obj != null)
            {
                //Debug.Log("Found object at (0,0): " + obj.name);
            }
        }
        else
        {
            Debug.LogError("GridManager or grid is not set up!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CheckGridForNull();
        }
        
    }

    private void CheckGridForNull()
    {

        for (int y = 0; y < GridManager.rows; y++)
        {
            for (int x = 0; x < GridManager.cols; x++)
            {
                if (gridManager.isAlive[y, x] == false &&
                    gridManager.isAlive[y + 1, x] == true)
                {
                    Debug.Log("Null grid element detected at [" + y + "," + x + "]");
                    StartCoroutine(DropBlock(y, x));
                    GridManager.gridUpdateHappened = true;
                }
            }
        }

    }

    private System.Collections.IEnumerator DropBlock(int y, int x)
    {
        if (gridManager.grid[y + 1, x] == null) yield break;
        //if (gridManager.grid[y, x] != null) yield break;
        //isMoving = true;

        //getting reference to the block GameObject
        GameObject obj = gridManager.grid[y + 1, x];
        //Debug.Log("Found object at (y + 1,x): " + obj.name);

        //getting GameObject position
        Vector3 startPos = obj.transform.position;

        //getting new position
        //Vector3 destinationPos1 = new Vector3(obj1.transform.position.x + 1, obj1.transform.position.y, obj1.transform.position.z);
        Vector3 finishPos = new Vector3(obj.transform.position.x, obj.transform.position.y - 1, obj.transform.position.z);

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * GridManager.swapSpeed;
            obj.transform.position = Vector3.Lerp(startPos, finishPos, progress);
            yield return null;
        }



        //obj.transform.position = finishPos;

        // Update the grid to reflect the new position
        //grid[x, y] = null;

        // Calculate new grid position (if necessary)
        //Vector2Int newGridIndex = GetGridIndexFromPosition(newPosition);
        //grid[newGridIndex.x, newGridIndex.y] = obj;

        gridManager.grid[y, x] = obj;
        gridManager.isAlive[y, x] = true;
        gridManager.isAlive[y + 1, x] = false;

        Debug.Log("gridUpdateHappened = true");
        GridManager.gridUpdateHappened = true;
    }
}
