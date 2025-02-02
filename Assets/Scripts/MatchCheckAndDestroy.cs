using System.Collections.Generic;
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
        //if (GridManager.gridUpdateHappened)
        //{
        //    MatchFindAndDestroy();
        //    GridManager.gridUpdateHappened = false;
        //}
        if (Input.GetKeyDown(KeyCode.H))
        {
            MatchFindAndDestroy();
        }
    }

    void MatchFindAndDestroy()
    {
        bool allElemsAreNotNull = true;
        int colorVar = 1;
        List<Vector2Int> matchesToDestroy = new List<Vector2Int>();

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
                for (int x = 1; x < GridManager.cols; x++)
                {
                    //Debug.Log("Checking[" + y + "," + x + "]");

                    if (gridManager.grid[y, x] != null && gridManager.grid[y, x - 1] != null && 
                        gridManager.grid[y, x].tag.Equals(gridManager.grid[y, x - 1].tag))
                    {
                        colorVar++;
                        if (colorVar >= 3)
                        {
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            Debug.Log("------------");
                            Debug.Log("Match Found!");
                            Debug.Log("[" + y + "," + (x - 2) + "], tag: " + gridManager.grid[y, x - 2].tag);
                            Debug.Log("[" + y + "," + (x - 1) + "], tag: " + gridManager.grid[y, x - 1].tag);
                            Debug.Log("[" + y + "," + x + "], tag: " + gridManager.grid[y, x].tag);
                            Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            matchesToDestroy.Add(new Vector2Int(y, x));
                            matchesToDestroy.Add(new Vector2Int(y, x - 1));
                            matchesToDestroy.Add(new Vector2Int(y, x - 2));
                            gridManager.isAlive[y, x - 2] = false;
                            gridManager.isAlive[y, x - 1] = false;
                            gridManager.isAlive[y, x] = false;
                            colorVar = 1;
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
                for (int y = 1; y < GridManager.rows; y++)
                {
                    //Debug.Log("Checking[" + y + "," + x + "]");
                    if (gridManager.grid[y, x] != null && gridManager.grid[y - 1, x] != null && 
                        gridManager.grid[y, x].tag.Equals(gridManager.grid[y - 1, x].tag))
                    {
                        colorVar++;
                        if (colorVar >= 3)
                        {
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            Debug.Log("------------");
                            Debug.Log("Match Found!");
                            Debug.Log("[" + (y - 2) + "," + x + "], tag: " + gridManager.grid[y - 2, x].tag);
                            Debug.Log("[" + (y - 1) + "," + x + "], tag: " + gridManager.grid[y - 1, x].tag);
                            Debug.Log("[" + y + "," + x + "], tag: " + gridManager.grid[y, x].tag);
                            Debug.Log("------------");
                            //Debug.Log("------------");
                            //Debug.Log("------------");
                            matchesToDestroy.Add(new Vector2Int(y, x));
                            matchesToDestroy.Add(new Vector2Int(y - 1, x));
                            matchesToDestroy.Add(new Vector2Int(y - 2, x));
                            gridManager.isAlive[y - 2, x] = false;
                            gridManager.isAlive[y - 1, x] = false;
                            gridManager.isAlive[y, x] = false;
                            colorVar = 1;
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
        if (matchesToDestroy.Count > 0)
        {
            foreach (var pos in matchesToDestroy)
            {
                if (gridManager.grid[pos.x, pos.y] != null)
                {
                    Destroy(gridManager.grid[pos.x, pos.y]);
                    gridManager.isAlive[pos.x, pos.y] = false;
                    gridManager.grid[pos.x, pos.y] = null;
                }
            }

            GridManager.gridDestroyHappened = true;
        }   
    }



}
