using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private Cell[,] allTiles;

    public bool IsBusy { get; private set; }

    void Start()
    {
        allTiles = new Cell[width, height];
        PlayAreaMask.Create(transform, width, height);
        SetUp();
    }

    #region Setup

    private void SetUp()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = SpawnCell(x, y, new Vector2(x, y));
                cell.Type = PickTypeWithoutMatch(x, y, cell.dots.Length);
            }
        }
    }

    // Relies on the fill order (left-to-right, bottom-to-top): only the two cells to the left
    // and the two below exist yet, so those are the only ones that can complete a match
    private TileType PickTypeWithoutMatch(int x, int y, int typeCount)
    {
        TileType? bannedLeft = null;
        TileType? bannedBelow = null;

        if (x >= 2 && allTiles[x - 1, y].Type == allTiles[x - 2, y].Type)
            bannedLeft = allTiles[x - 1, y].Type;
        if (y >= 2 && allTiles[x, y - 1].Type == allTiles[x, y - 2].Type)
            bannedBelow = allTiles[x, y - 1].Type;

        TileType type;
        do
        {
            type = (TileType)Random.Range(0, typeCount);
        } while (type == bannedLeft || type == bannedBelow);

        return type;
    }

    private Cell SpawnCell(int x, int y, Vector2 spawnPosition)
    {
        GameObject cellObject = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform);
        cellObject.name = "(" + x + ", " + y + ")";

        Cell cell = cellObject.GetComponent<Cell>();
        cell.x = x;
        cell.y = y;
        allTiles[x, y] = cell;
        return cell;
    }

    #endregion

    #region Player input

    public void RemoveCell(Cell cell)
    {
        if (cell == null) return;
        if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height) return;
        if (allTiles[cell.x, cell.y] != cell) return;

        allTiles[cell.x, cell.y] = null;
        StartCoroutine(ResolveBoard());
    }

    #endregion

    #region Resolving the board

    // Loops because refilled cells can form new matches (cascades)
    private IEnumerator ResolveBoard()
    {
        IsBusy = true;

        do
        {
            for (int x = 0; x < width; x++)
            {
                ApplyGravity(x);
                RefillColumn(x);
            }

            while (AnyCellMoving()) yield return null;
        }
        while (ClearMatches());

        IsBusy = false;
    }

    private void ApplyGravity(int x)
    {
        int writeY = 0;
        for (int readY = 0; readY < height; readY++)
        {
            Cell cell = allTiles[x, readY];
            if (cell == null) continue;

            if (readY != writeY)
            {
                allTiles[x, readY] = null;
                allTiles[x, writeY] = cell;
                cell.ChangePosition(x, writeY);
            }
            writeY++;
        }
    }

    // Stacked above the top row so new cells don't overlap while falling in
    private void RefillColumn(int x)
    {
        int spawnOffset = 0;
        for (int y = 0; y < height; y++)
        {
            if (allTiles[x, y] != null) continue;

            Cell cell = SpawnCell(x, y, new Vector2(x, height + spawnOffset));
            cell.Type = (TileType)Random.Range(0, cell.dots.Length);
            cell.ChangePosition(x, y);
            spawnOffset++;
        }
    }

    private bool ClearMatches()
    {
        bool[,] matched = MatchFinder.FindMatches(allTiles);
        bool any = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!matched[x, y]) continue;

                any = true;
                Destroy(allTiles[x, y].gameObject);
                allTiles[x, y] = null;
            }
        }

        return any;
    }

    private bool AnyCellMoving()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allTiles[x, y] != null && allTiles[x, y].IsMoving) return true;
            }
        }
        return false;
    }

    #endregion
}
