using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

/*
 TODO 
one prefab array instead of separate prefabs
match 4, match 5
do not allow player to select invisible blocks at the top
 */

public class GridManager : MonoBehaviour
{
    private float timer = 0f;
    private float interval = 6f;

    //public GameObject GameObjectToDestroy;
    public GameObject[,] grid;
    public bool[,] isAlive;
    //public GameObject[] prefabToSpawn;
    public GameObject prefabToSpawnGreen;
    public GameObject prefabToSpawnRed;
    public GameObject prefabToSpawnBlue;
    public GameObject prefabToSpawnYellow;
    public static int rows = 3;
    public static int cols = 5;
    private DetectClick currentSelection = null;

    public Vector2 spawnPosition;

    private bool isSwapping = false; // To prevent multiple swaps simultaneously
    private GameObject selectedObject = null; // Tracks the first object selected
    public static float swapSpeed = 5f;

    public static bool gridUpdateHappened = false;
    public static bool gridDestroyHappened = false;

    private void Awake()
    {
        grid = new GameObject[rows + 1, cols];
        isAlive = new bool[rows + 1, cols];

        do
        {
            if (grid[0, 0] != null)
            {
                DestroyGrid();
            }
            PopulateGrid();
        }
        while (MatchFound() == true);
    }

    void Start()
    {
        //InvokeRepeating("CheckGridForNull", 2.0f, 5f);

        //grid = new GameObject[cols, rows];
        //grid = new GameObject[rows + 1, cols];

        //do
        //{
        //    if (grid[0, 0] != null)
        //    {
        //        DestroyGrid();
        //    }
        //    PopulateGrid();
        //}
        //while (MatchFound() == true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int clickedIndex = GetGridIndexFromPosition(mousePosition);
            //Debug.Log($"Mouse clicked at: {mousePosition}, Grid Index: {clickedIndex}");

            if (AreIndicesValid(clickedIndex, clickedIndex))
            {
                HandleSelection(clickedIndex);
            }
        }

        //moved to separate script
        //if (gridUpdateHappened) 
        //{
        //    MatchFindAndDestroy();
        //    gridUpdateHappened = false;
        //}

        if (gridDestroyHappened)
        {
            SpawnNewBlockAtTheTop();
            gridDestroyHappened = false;
        }

        //CheckGridForNull();

    }

    private void CheckGridForNull()
    {

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {

                if (isAlive[y, x] == false)
                {
                    Debug.Log("Null grid element detected at [" + y + "," + x + "]");
                }

                //Debug.Log("grid element status at [" + y + "," + x + "] is " + isAlive[y, x]);
            }
        }
    }

    //void SpawnPreset1()
    //{
    //    grid[0, 0] = Instantiate(prefabToSpawnGreen, spawnPosition, Quaternion.identity); spawnPosition.x += 1;
    //    grid[0, 1] = Instantiate(prefabToSpawnRed, spawnPosition, Quaternion.identity); spawnPosition.x += 1;
    //    grid[0, 2] = Instantiate(prefabToSpawnRed, spawnPosition, Quaternion.identity); spawnPosition.x = 0.5f; spawnPosition.y += 1;
    //    grid[1, 0] = Instantiate(prefabToSpawnRed, spawnPosition, Quaternion.identity); spawnPosition.x += 1;
    //    grid[1, 1] = Instantiate(prefabToSpawnBlue, spawnPosition, Quaternion.identity); spawnPosition.x += 1;
    //    grid[2, 2] = Instantiate(prefabToSpawnGreen, spawnPosition, Quaternion.identity); spawnPosition.x = 0.5f; spawnPosition.y += 1;
    //    grid[2, 0] = Instantiate(prefabToSpawnRed, spawnPosition, Quaternion.identity); spawnPosition.x += 1;
    //    grid[2, 1] = Instantiate(prefabToSpawnGreen, spawnPosition, Quaternion.identity); spawnPosition.x += 1;
    //    grid[2, 2] = Instantiate(prefabToSpawnRed, spawnPosition, Quaternion.identity); spawnPosition.x = 0.5f; spawnPosition.y += 1;
    //}

    void DestroyGrid()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Destroy(grid[y, x]);
            }
        }
    }

    void PopulateGrid()
    {


        for (int y = 0; y < rows + 1; y++)
        {
            for (int x = 0; x < cols; x++)
            {

                if (y < rows)
                {
                    grid[y, x] = Instantiate(GetRandomPrefab(), spawnPosition, Quaternion.identity);
                    //Debug.Log("Spawned grid[" + y + "," + x + "]");

                    spawnPosition.x += 1;
                    //Debug.Log("spawnPosition.x = " + spawnPosition.x);

                    isAlive[y, x] = true;
                }
                else 
                {
                    //grid[y, x] = null;
                }
            }
            spawnPosition.x = 0.5f;
            spawnPosition.y += 1;
            //Debug.Log("spawnPosition.y = " + spawnPosition.y);
        }
        spawnPosition.y = 0.5f;
    }

    bool MatchFound()
    {
        //horizontal matches
        int rowCounter = 1;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 1; x < cols; x++)
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
        for (int x = 0; x < cols; x++)
        {
            for (int y = 1; y < rows; y++)
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

    //void MatchFindAndDestroy()
    //{
    //    bool allElemsAreNotNull = true;
    //    int colorVar = 1;

    //    for (int y = 0; y < rows; y++)
    //    {
    //        for (int x = 0; x < cols; x++)
    //        {
    //            if (grid[y, x] == null)
    //            {
    //                allElemsAreNotNull = false;
    //            }
    //        }
    //    }

    //    if (allElemsAreNotNull)
    //    {

    //        //horizontal matches
    //        for (int y = 0; y < rows; y++)
    //        {
    //            for (int x = 0; x < cols; x++)
    //            {
    //                //Debug.Log("Checking[" + y + "," + x + "]");

    //                if (x >= 1 && grid[y, x].tag.Equals(grid[y, x - 1].tag))
    //                {
    //                    colorVar++;
    //                    if (colorVar == 3)
    //                    {
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("Match Found!");
    //                        //Debug.Log("[" + y + "," + x + "]");
    //                        //Debug.Log("and prev 2 by x axis");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        Destroy(grid[y, x - 2]);
    //                        StartCoroutine(DropBlock(y, x - 2));
    //                        Destroy(grid[y, x - 1]);
    //                        StartCoroutine(DropBlock(y, x - 1));
    //                        Destroy(grid[y, x]);
    //                        StartCoroutine(DropBlock(y, x));
    //                        //Destroy(grid[0, 0]);
    //                        //grid[y, x] = null;
    //                        colorVar = 1;
    //                        gridDestroyHappened = true;
    //                    }
    //                }
    //                else
    //                {
    //                    colorVar = 1;
    //                }
    //            }
    //            colorVar = 1;
    //        }

    //        //vertical matches
    //        //int colCounter = 1;
    //        for (int x = 0; x < cols; x++)
    //        {
    //            for (int y = 0; y < rows; y++)
    //            {
    //                //Debug.Log("Checking[" + y + "," + x + "]");
    //                if (y >= 1 && grid[y, x].tag.Equals(grid[y - 1, x].tag))
    //                {
    //                    colorVar++;
    //                    if (colorVar == 3)
    //                    {
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("Match Found!");
    //                        //Debug.Log("[" + y + "," + x + "]");
    //                        //Debug.Log("and prev 2 by y axis");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        //Debug.Log("------------");
    //                        //return true;
    //                        Destroy(grid[y - 2, x]);
    //                        Destroy(grid[y - 1, x]);
    //                        Destroy(grid[y, x]);
    //                        colorVar = 1;
    //                        gridDestroyHappened = true;
    //                    }
    //                }
    //                else
    //                {
    //                    colorVar = 1;
    //                }
    //            }
    //            colorVar = 1;
    //        }
    //    }
    //}
    
    void DropUpperGameObject(int y, int x)
    {
        //getting reference to the block GameObject
        GameObject obj1 = grid[y + 1, x];

        //getting GameObject position
        Vector3 startPos1 = obj1.transform.position;

        //getting new position
        //Vector3 destinationPos1 = new Vector3(obj1.transform.position.x + 1, obj1.transform.position.y, obj1.transform.position.z);
        Vector3 finishPos = new Vector3(obj1.transform.position.x, obj1.transform.position.y - 1, obj1.transform.position.z);

        // Ensure we're within bounds and there is a GameObject above the current position
        if (y + 1 < grid.GetLength(0) && grid[y + 1, x] != null)
        {
            // Move the upper GameObject to the current position
            grid[y, x] = obj1;
            //grid[y + 1, x] = null;

            // Optionally, update the GameObject's position in the scene
            //Vector3 newPosition = new Vector3(x, y, 0); // Adjust z-axis if necessary
            obj1.transform.position = finishPos;
        }
        else
        {
            // No upper GameObject exists or out of bounds
            //grid[y, x] = null;
        }
    }

    void SpawnNewBlockAtTheTop() 
    {
        float spawnPosX = 0.5f;
        float spawnPosY = rows + 0.5f;
        Vector2 spawnPositionAdditional = new Vector2(spawnPosX, spawnPosY);
        int indexY = rows;
        //int indexX = 0;
        for (int q = 0; q < cols; q++)
        {
            GameObject newBlock = Instantiate(GetRandomPrefab(), spawnPositionAdditional, Quaternion.identity);

            // Make it invisible by disabling the SpriteRenderer
            SpriteRenderer spriteRenderer = newBlock.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true; // Turn off visibility'
            }

            grid[indexY, q] = newBlock;
            spawnPositionAdditional.x = spawnPosX + (q + 1);
        }
        Debug.Log("blocks have spawned");
    }

    private System.Collections.IEnumerator DropBlock(int y, int x/*Vector2Int index*//*, Vector2 newPosition*/)
    {
        //isMoving = true;

        //getting reference to the block GameObject
        GameObject obj = grid[y + 1, x];

        //getting GameObject position
        Vector3 startPos = obj.transform.position;

        //getting new position
        //Vector3 destinationPos1 = new Vector3(obj1.transform.position.x + 1, obj1.transform.position.y, obj1.transform.position.z);
        Vector3 finishPos = new Vector3(obj.transform.position.x, obj.transform.position.y - 1, obj.transform.position.z);

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * swapSpeed;
            obj.transform.position = Vector3.Lerp(startPos, finishPos, progress);
            yield return null;
        }

        grid[y, x] = obj;

        obj.transform.position = finishPos;

        // Update the grid to reflect the new position
        //grid[x, y] = null;

        // Calculate new grid position (if necessary)
        //Vector2Int newGridIndex = GetGridIndexFromPosition(newPosition);
        //grid[newGridIndex.x, newGridIndex.y] = obj;

        //isMoving = false;
        //Debug.Log("gridUpdateHappened = true");
        gridUpdateHappened = true;
    }

    GameObject GetRandomPrefab()
    {
        int random = UnityEngine.Random.Range(0, 4);
        if (random == 0)
        {
            return prefabToSpawnBlue;
        }
        else if (random == 1)
        {
            return prefabToSpawnGreen;
        }
        else if (random == 2)
        {
            return prefabToSpawnRed;
        }
        else
        {
            return prefabToSpawnYellow;
        }
        //return prefabToSpawn[random];
    }

    void HandleSelection(Vector2Int clickedIndex)
    {
        if (selectedObject == null)
        {
            //I don't know why x and y are reversed here but it works
            //Debug.Log("First object selected grid[" + clickedIndex.x + "," + clickedIndex.y + "]");
            selectedObject = grid[clickedIndex.x, clickedIndex.y];
        }
        else
        {
            //Debug.Log("Second object selected, swapping " + GetGridIndexFromPosition(selectedObject.transform.position));
            Vector2Int firstIndex = GetGridIndexFromPosition(selectedObject.transform.position);
            if(ObjectsPositionsAreValidForSwap(firstIndex, clickedIndex))
            {
                SwapObjects(firstIndex, clickedIndex);
            }
            selectedObject = null;
        }
        //Debug.Log("gridUpdateHappened = true");
        //gridUpdateHappened = true;
    }

    Vector2Int GetGridIndexFromPosition(Vector2 position)
    {
        int y = Mathf.FloorToInt(position.y);
        int x = Mathf.FloorToInt(position.x);
        return new Vector2Int(y, x);
    }

    public void SwapObjects(Vector2Int index1, Vector2Int index2)
    {
        if (isSwapping)
        {
            Debug.Log("Swap in progress, cannot start another swap.");
            return;
        }

        if (AreIndicesValid(index1, index2))
        {
            StartCoroutine(SwapGridObjects(index1, index2));
        }
        else
        {
            Debug.LogError("Invalid indices provided for swapping.");
        }
    }

    private bool AreIndicesValid(Vector2Int index1, Vector2Int index2)
    {
        //I don't know why x and y are reversed here but it works
        //Should be: The first dimension (0) corresponds to the rows, i.e., the vertical axis (cols). y
        //Should be: The second dimension (1) corresponds to the horizontal axis (rows). x
        return index1.x >= 0 && index1.x < grid.GetLength(0) && 
               index1.y >= 0 && index1.y < grid.GetLength(1) && 
               index2.x >= 0 && index2.x < grid.GetLength(0) &&
               index2.y >= 0 && index2.y < grid.GetLength(1);
    }

    private System.Collections.IEnumerator SwapGridObjects(Vector2Int index1, Vector2Int index2)
    {
        isSwapping = true;

        //I don't know why x (cols) and y (rows) are reversed here but it works
        GameObject obj1 = grid[index1.x, index1.y];
        GameObject obj2 = grid[index2.x, index2.y];

        if (obj1 == null || obj2 == null)
        {
            Debug.LogError("One or both objects to swap are null.");
            isSwapping = false;
            yield break;
        }

        Vector3 startPos1 = obj1.transform.position;
        Vector3 startPos2 = obj2.transform.position;

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * swapSpeed;
            obj1.transform.position = Vector3.Lerp(startPos1, startPos2, progress);
            obj2.transform.position = Vector3.Lerp(startPos2, startPos1, progress);
            yield return null;
        }

        obj1.transform.position = startPos2;
        obj2.transform.position = startPos1;

        //I don't know why x and y are reversed here but it works
        grid[index1.x, index1.y] = obj2;
        grid[index2.x, index2.y] = obj1;

        isSwapping = false;
        //Debug.Log("gridUpdateHappened = true");
        gridUpdateHappened = true;
    }

    bool ObjectsPositionsAreValidForSwap(Vector2Int first, Vector2Int second)
    {
        bool xAreEquals = false;
        bool yAreEquals = false;
        if (first.x == second.x)
        {
            xAreEquals = true;
        }
        if (first.y == second.y)
        {
            yAreEquals = true;
        }
        if (first.x - second.x == 1 && yAreEquals ||
            first.x - second.x == -1 && yAreEquals ||
            first.y - second.y == 1 && xAreEquals ||
            first.y - second.y == -1 && xAreEquals)
        {
            return true;
        }
        return false;
    }



}

/*

 
*/
