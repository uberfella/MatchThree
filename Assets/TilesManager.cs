using Unity.VisualScripting;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    private Cell[,] board;
    //private TileView[,] views;
    private Tile[,] views;
    private int cols = 3;
    private int rows = 5;
    private Tile tile;
    //private Vector2 spawnPosition;
    [SerializeField] private Tile tilePrefab;

    void Start()
    {
        //spawnPosition.x = 0.5f;
        //spawnPosition.y = 0.5f;
        board = new Cell[cols, rows];
        views = new Tile[cols, rows];
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
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {

                Cell cell = new Cell();

                board[x, y] = cell;

                board[x, y].Type = (TileType)UnityEngine.Random.Range(0, 4);


            }
        }
    }

    void InstantiateTiles()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                 tile = Instantiate(tilePrefab);

                tile.Init(x, y, this);

                //tile.transform.position = new Vector3(
                //    x,
                //    y,
                //    0);
                UpdateWorldPos(x, y, tile);

                views[x, y] = tile;
            }
        }
    }

    void UpdateVisuals()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                //if (views[y, x] == null)
                //    Debug.Log($"views[{y},{x}] is null");

                //if (board[y, x] == null)
                //    Debug.Log($"board[{y},{x}] is null");

                if (views[x, y] == null)
                {
                    continue;
                }

                if (board[x, y] == null)
                {
                    continue;
                }

                views[x, y].SetSprite(board[x, y].Type);
                UpdateWorldPos(x, y, tile);

            }
        }

    }

    public void OnTileClicked(int x, int y, GameObject gameObject)
    {
        Debug.Log("before - board[y, x] = " + board[x, y]);

        //Cell cell = board[y, x];
        Tile tile = views[x, y];
        Debug.Log("this is " + tile.X + " " + tile.Y);
        Debug.Log("this is " + views[x, y]+ " with x = "+x +" and y = "+ y);
        Debug.Log(" " );

        //Destroy(tile);
        //


        IDestroyable target = gameObject.GetComponent<IDestroyable>();

        if (target != null)
        {
            target.Destroy();
            Destroy(gameObject);
        }
        board[x, y] = null;
        views[x, y] = null;
        //Debug.Log("board[y, x] = " + board[y, x]);
        //Debug.Log(" ");
        //Debug.Log("OnBoardChanged(); ");
        //Debug.Log(" ");
        OnBoardChanged();
    }

    private void OnBoardChanged()
    {
        ApplyGravity();
        UpdateVisuals();
    }

    void ApplyGravity()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows-1; y++)
            {
                if (board[x, y] == null && board[x, y + 1] != null)
                {
                    Debug.Log(" ");
                    Debug.Log("found null, getting down");
                    Debug.Log(" ");

                    board[x, y] = board[x, y + 1];
                    board[x, y + 1] = null;
                }
            }
        }
    }

    void UpdateWorldPos(int x, int y, Tile tile)
    {
        tile.transform.position = new Vector3(
            x,
            y,
            0);
    }
}
