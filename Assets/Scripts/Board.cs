using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        SetUp();
    }


    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject cell = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                cell.transform.parent = this.transform;
                cell.name = "(" + i + ", " + j + ")";

            }
        }
    }

    void Update()
    {
        
    }
}
