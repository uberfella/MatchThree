using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
//TODO 4 blocks match

public class MatchCheckAndDestroy : MonoBehaviour
{
    public GameObject tileBlue;
    private GridManager gridManager;
    //private GameObject[,] grid;
    //public StartGame startGame;
    //private int cols;
    //private int rows;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if (GridManager.gridUpdateHappened)
        {
            MatchFindAndDestroy();
            GridManager.gridUpdateHappened = false;
        }
    }

    void MatchFindAndDestroy()
    {
        bool allElemsAreNotNull = true;
        int colorVar = 1;

        for (int y = 0; y < GridManager.rows; y++)
        {
            for (int x = 0; x < GridManager.cols; x++)
            {
                if (gridManager.grid[y, x] == null)
                {
                    allElemsAreNotNull = false;
                }
            }
        }

        if (allElemsAreNotNull)
        {

            //horizontal matches
            for (int y = 0; y < GridManager.rows; y++)
            {
                for (int x = 0; x < GridManager.cols; x++)
                {
                    //Debug.Log("Checking[" + y + "," + x + "]");

                    if (x >= 1 && gridManager.grid[y, x].tag.Equals(gridManager.grid[y, x - 1].tag))
                    {
                        colorVar++;
                        if (colorVar == 3)
                        {
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("Match Found!");
                            //Debug.Log("[" + y + "," + x + "]");
                            //Debug.Log("and prev 2 by x axis");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            Destroy(gridManager.grid[y, x - 2]);
                            //StartCoroutine(DropBlock(y, x - 2));
                            Destroy(gridManager.grid[y, x - 1]);
                            //StartCoroutine(DropBlock(y, x - 1));
                            Destroy(gridManager.grid[y, x]);
                            //StartCoroutine(DropBlock(y, x));
                            //Destroy(grid[0, 0]);
                            //gridManager.grid[y, x] = null;
                            gridManager.isAlive[y, x - 2] = false;
                            gridManager.isAlive[y, x - 1] = false;
                            gridManager.isAlive[y, x] = false;
                            colorVar = 1;
                            GridManager.gridDestroyHappened = true;
                        }
                    }
                    else
                    {
                        colorVar = 1;
                    }
                }
                colorVar = 1;
            }

            //vertical matches
            //int colCounter = 1;
            for (int x = 0; x < GridManager.cols; x++)
            {
                for (int y = 0; y < GridManager.rows; y++)
                {
                    //Debug.Log("Checking[" + y + "," + x + "]");
                    if (y >= 1 && gridManager.grid[y, x].tag.Equals(gridManager.grid[y - 1, x].tag))
                    {
                        colorVar++;
                        if (colorVar == 3)
                        {
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("Match Found!");
                            //Debug.Log("[" + y + "," + x + "]");
                            //Debug.Log("and prev 2 by y axis");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            //return true;
                            Destroy(gridManager.grid[y - 2, x]);
                            Destroy(gridManager.grid[y - 1, x]);
                            Destroy(gridManager.grid[y, x]);
                            colorVar = 1;
                            GridManager.gridDestroyHappened = true;
                        }
                    }
                    else
                    {
                        colorVar = 1;
                    }
                }
                colorVar = 1;
            }
        }
    }

    
}
