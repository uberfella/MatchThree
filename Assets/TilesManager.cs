using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    private Tile[,] grid;
    private static int rows = 3;
    private static int cols = 5;
    private Vector2 spawnPosition;

    void Start()
    {
        spawnPosition.x = 0.5f;
        spawnPosition.y = 0.5f;
        grid = new Tile[rows + 1, cols];
        PopulateGrid();
    }

    void Update()
    {
        
    }

    void PopulateGrid()
    {
        for (int y = 0; y < rows + 1; y++)
        {
            for (int x = 0; x < cols; x++)
            {

                if (y < rows)
                {
                    //grid[y, x] = Instantiate(GetRandomPrefab(), spawnPosition, Quaternion.identity);
                    //grid[y, x] = Instantiate(prefabsToSpawn[0], spawnPosition, Quaternion.identity);
                    GameObject newTile = Instantiate(prefabsToSpawn[0], spawnPosition, Quaternion.identity);
                    grid[y, x] = newTile;
                    //Debug.Log("Spawned grid[" + y + "," + x + "]");

                    spawnPosition.x += 1;
                    //Debug.Log("spawnPosition.x = " + spawnPosition.x);

                    //isAlive[y, x] = true;
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
}
