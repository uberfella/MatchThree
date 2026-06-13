using Unity.VisualScripting;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    private Cell[,] board;
    //private TileView[,] views;
    private Tile [,] views;
    private int rows = 3;
    private int cols = 5;
    //private Vector2 spawnPosition;
    [SerializeField] private Tile tilePrefab;

    void Start()
    {
        //spawnPosition.x = 0.5f;
        //spawnPosition.y = 0.5f;
        board = new Cell[cols, rows];
        //views = new TileView[cols, rows];
        views = new Tile [cols, rows];
        PopulateBoard();
        InstantiateTiles();
        UpdateVisuals();
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) 
        {
        }

    }

    void PopulateBoard()
    {
        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {

                //if (y < rows)
                {
                    //grid[y, x] = Instantiate(GetRandomPrefab(), spawnPosition, Quaternion.identity);
                    //grid[y, x] = newTile;

                    board[y, x] = new Cell();

                    //board[y, x].Type = TileType.Red;
                    board[y, x].Type = (TileType)UnityEngine.Random.Range(0, 4);

                    //Tile tile = Instantiate(Type);

                    //Debug.Log("Spawned board[" + y + "," + x + "]" + ", type = "+ board[y, x].Type);
                    //spawnPosition.x += 1;
                    //Debug.Log("spawnPosition.x = " + spawnPosition.x);

                    //isAlive[y, x] = true;
                }
                //else
                //{
                //    //grid[y, x] = null;
                //}
            }
            //spawnPosition.x = 0.5f;
            //spawnPosition.y += 1;
            //Debug.Log("spawnPosition.y = " + spawnPosition.y);
            //Debug.Log(" ");
        }
        //spawnPosition.y = 0.5f;
    }

    void InstantiateTiles()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Tile tile = Instantiate(tilePrefab);

                tile.Init(x, y, this);

                tile.transform.position = new Vector3(
                    x,
                    y,
                    0);

                views[y, x] = tile;
            }
        }
    }

    void UpdateVisuals()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                //if (views[y, x] == null)
                //    Debug.Log($"views[{y},{x}] is null");

                //if (board[y, x] == null)
                //    Debug.Log($"board[{y},{x}] is null");
                views[y, x].SetSprite(board[y, x].Type);
            }
        }
    }

    public void OnTileClicked(int x, int y, GameObject gameObject)
    {
        //Cell cell = board[y, x];
        Tile tile = views[y, x];
        Debug.Log("this is " + tile.X + " " + tile.Y);

        //Destroy(tile);
        //


        IDestroyable target = gameObject.GetComponent<IDestroyable>();

        if (target != null)
        {
            target.Destroy();
            Destroy(gameObject);
        }
        board[y, x] = null;

    }

}
