using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class MatchCheckAndDestroy : MonoBehaviour
{
    public GameObject tileBlue;
    private GridManager gridManager;
    private GameObject[,] grid;
    //public StartGame startGame;
    public int cols;
    public int rows;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool MatchFound()
    {
        //horizontal matches
        int rowCounter = 1;
        for (int y = 0; y < cols; y++)
        {
            for (int x = 1; x < rows; x++)
            {
                if (grid[y, x].tag.Equals(grid[y, x - 1].tag))
                {
                    rowCounter++;
                    if (rowCounter == 3)
                    {
                        //Debug.Log(" horizontal matchFound!!! ");
                        return true;
                    }
                }
                else
                {
                    rowCounter = 1;
                }
            }
            rowCounter = 1;
        }

        //vertical matches
        int colCounter = 1;
        for (int x = 0; x < rows; x++)
        {
            for (int y = 1; y < cols; y++)
            {
                if (grid[y, x].tag.Equals(grid[y - 1, x].tag))
                {
                    colCounter++;
                    if (colCounter == 3)
                    {
                        //Debug.Log(" vertical matchFound!!! ");
                        return true;
                    }
                }
                else
                {
                    colCounter = 1;
                }
            }
            colCounter = 1;
        }
        return false;
    }

    void MatchFindAndDestroy()
    {
        bool allElemsAreNotNull = true;

        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                if (grid[y, x] == null)
                {
                    allElemsAreNotNull = false;
                }
            }
        }

        if (allElemsAreNotNull)
        {

            //horizontal matches
            int rowCounter = 1;
            for (int y = 0; y < cols; y++)
            {
                for (int x = 1; x < rows - 1; x++)
                {
                    Debug.Log("Checking[" + y + "," + x + "]");
                    if (grid[y, x].tag.Equals(grid[y, x + 1].tag))
                    {
                        rowCounter++;
                        if (rowCounter == 2)
                        {
                            Debug.Log("------------");
                            Debug.Log("Match FOund!");
                            Debug.Log("[" + y + "," + x + "]");
                            Debug.Log("------------");
                            //Destroy(grid[y, x - 2]);
                            //Destroy(grid[y, x - 1]);
                            //Destroy(grid[y, x]);
                            //Destroy(grid[0, 0]);
                            //grid[y, x] = null;
                        }
                    }
                    else
                    {
                        rowCounter = 0;
                    }
                }
                rowCounter = 0;
            }

            //vertical matches
            //int colCounter = 1;
            //for (int x = 0; x < rows; x++)
            //{
            //    for (int y = 0; y < cols; y++)
            //    {
            //        if (grid[y, x].tag.Equals(grid[y - 1, x].tag))
            //        {
            //            colCounter++;
            //            if (colCounter == 3)
            //            {
            //                //return true;
            //                //Destroy(grid[y - 2, x]);
            //                //Destroy(grid[y - 1, x]);
            //                //Destroy(grid[y, x]);
            //            }
            //        }
            //        else
            //        {
            //            colCounter = 1;
            //        }
            //    }
            //    colCounter = 1;
            //}
        }
    }
}
