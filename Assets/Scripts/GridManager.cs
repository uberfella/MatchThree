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

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int clickedIndex = GetGridIndexFromPosition(mousePosition);

            if (AreIndicesValid(clickedIndex, clickedIndex))
            {
                HandleSelection(clickedIndex);
            }
        }
            
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    SpawnNewBlockAtTheTop();
        //}
        SpawnNewBlockAtTheTop();

        /*
             
         
        */

    }

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

    void SpawnNewBlockAtTheTop()
    {
        float spawnPosX = 0.5f;
        float spawnPosY = rows + 0.5f;
        Vector2 spawnPositionAdditional = new Vector2(spawnPosX, spawnPosY);
        int indexY = rows;
        for (int q = 0; q < cols; q++)
        {
            if (isAlive[indexY, q] == false)
            {
                Debug.Log("isAlive[" + indexY + "," + q+ "] == " + isAlive[indexY, q]);
                grid[indexY, q] = Instantiate(GetRandomPrefab(), spawnPositionAdditional, Quaternion.identity);
                isAlive[indexY, q] = true;
            }
            spawnPositionAdditional.x = spawnPosX + (q + 1);
        }
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
            Debug.Log("First object selected grid[" + clickedIndex.x + "," + clickedIndex.y + "], isAlive status:" + isAlive[clickedIndex.x, clickedIndex.y]);
            selectedObject = grid[clickedIndex.x, clickedIndex.y];
        }
        else
        {
            Debug.Log("Second object selected, swapping " + GetGridIndexFromPosition(selectedObject.transform.position));
            Vector2Int firstIndex = GetGridIndexFromPosition(selectedObject.transform.position);
            if(ObjectsPositionsAreValidForSwap(firstIndex, clickedIndex))
            {
                SwapObjects(firstIndex, clickedIndex);
            }
            selectedObject = null;
        }
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
