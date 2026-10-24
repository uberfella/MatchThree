using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A cell resting on 'from' falls out of the board's bottom of its field and re-enters from above at 'to'.
// The slot 'to' has no spawner of its own while it is the end of a teleport.
[System.Serializable]
public struct Teleport
{
    public Vector2Int from;
    public Vector2Int to;
}

public class Board : MonoBehaviour
{
    public int width = 10;
    // Two fields (bottom and top) separated by a gap; each one has its own spawner along its top row.
    // Keep gapHeight >= fieldHeight so cells queued above the bottom field stay inside the gap, hidden by the masks
    public int fieldHeight = 3;
    public int gapHeight = 3;
    public GameObject tilePrefab;
    public List<Teleport> teleports = new List<Teleport>();
    private Cell[,] allTiles;
    // Teleport exits keyed by the slot they feed into
    private Dictionary<Vector2Int, Vector2Int> teleportSources = new Dictionary<Vector2Int, Vector2Int>();
    // Positions that are holes in the level: no cell is ever spawned there or moves through
    private bool[,] blocked;

    private int height => fieldHeight * 2 + gapHeight;

    public bool IsBusy { get; private set; }

    void Start()
    {
        allTiles = new Cell[width, height];
        blocked = new bool[width, height];
        BuildLayout();
        BuildTeleports();
        PlayAreaMask.Create(transform, width, 0, fieldHeight);
        PlayAreaMask.Create(transform, width, fieldHeight + gapHeight, fieldHeight);
        SetUp();
    }

    #region Setup

    private void BuildLayout()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = fieldHeight; y < fieldHeight + gapHeight; y++)
            {
                blocked[x, y] = true;
            }
        }
    }

    private void BuildTeleports()
    {
        foreach (Teleport teleport in teleports)
        {
            if (!IsPlayable(teleport.from) || !IsPlayable(teleport.to))
            {
                Debug.LogWarning($"Teleport {teleport.from} -> {teleport.to} ignored: both ends must be playable slots");
                continue;
            }
            if (!IsFieldTop(teleport.to.x, teleport.to.y))
            {
                Debug.LogWarning($"Teleport {teleport.from} -> {teleport.to} ignored: the entrance must be on the top row of a field");
                continue;
            }

            teleportSources[teleport.to] = teleport.from;
        }
    }

    private bool IsPlayable(Vector2Int p)
    {
        return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height && !blocked[p.x, p.y];
    }

    // The top row of a field: nothing above it, or a gap
    private bool IsFieldTop(int x, int y)
    {
        return y + 1 >= height || blocked[x, y + 1];
    }

    private void SetUp()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (blocked[x, y]) continue;

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

        if (x >= 2 && SameType(allTiles[x - 1, y], allTiles[x - 2, y]))
            bannedLeft = allTiles[x - 1, y].Type;
        if (y >= 2 && SameType(allTiles[x, y - 1], allTiles[x, y - 2]))
            bannedBelow = allTiles[x, y - 1].Type;

        TileType type;
        do
        {
            type = (TileType)Random.Range(0, typeCount);
        } while (type == bannedLeft || type == bannedBelow);

        return type;
    }

    // Gaps leave null neighbours, which can't form a match
    private static bool SameType(Cell a, Cell b)
    {
        return a != null && b != null && a.Type == b.Type;
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
            do
            {
                ApplyGravity();
            }
            while (RefillSpawners());

            while (AnyCellMoving()) yield return null;
        }
        while (ClearMatches());

        IsBusy = false;
    }

    // Straight down first; a cell only slides diagonally when the spot above the empty slot is a gap.
    // Left is tried before right. Repeats because every move opens a new empty slot above.
    private void ApplyGravity()
    {
        bool moved;
        do
        {
            moved = false;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (blocked[x, y] || allTiles[x, y] != null) continue;
                    if (TryFill(x, y)) moved = true;
                }
            }
        }
        while (moved);
    }

    private bool TryFill(int x, int y)
    {
        if (teleportSources.TryGetValue(new Vector2Int(x, y), out Vector2Int source))
            return TeleportCell(source, new Vector2Int(x, y));

        if (y + 1 >= height) return false;

        int above = y + 1;
        if (!blocked[x, above]) return MoveCell(x, above, x, y);
        return MoveCell(x - 1, above, x, y) || MoveCell(x + 1, above, x, y);
    }

    private bool MoveCell(int fromX, int fromY, int toX, int toY)
    {
        if (fromX < 0 || fromX >= width) return false;

        Cell cell = allTiles[fromX, fromY];
        if (cell == null) return false;

        allTiles[fromX, fromY] = null;
        allTiles[toX, toY] = cell;
        cell.ChangePosition(toX, toY);
        return true;
    }

    private bool TeleportCell(Vector2Int from, Vector2Int to)
    {
        Cell cell = allTiles[from.x, from.y];
        if (cell == null) return false;

        allTiles[from.x, from.y] = null;
        allTiles[to.x, to.y] = cell;
        cell.TeleportTo(to.x, to.y, new Vector2(from.x, from.y - 1), new Vector2(to.x, QueueY(to.x, to.y)));
        return true;
    }

    // Height just above a field's top row where a new cell waits, clear of cells already queued there
    private float QueueY(int x, int fieldTopY)
    {
        float queueY = fieldTopY + 1;
        for (int below = 0; below <= fieldTopY; below++)
        {
            Cell existing = allTiles[x, below];
            if (existing != null && existing.transform.position.y >= queueY)
                queueY = existing.transform.position.y + 1;
        }
        return queueY;
    }

    // Every field has a spawner on its top row: an empty slot with a gap (or the board edge) above it
    // gets a new cell, unless a teleport feeds that slot
    private bool RefillSpawners()
    {
        bool spawned = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (blocked[x, y] || allTiles[x, y] != null) continue;
                if (!IsFieldTop(x, y)) continue;
                if (teleportSources.ContainsKey(new Vector2Int(x, y))) continue;

                float spawnY = QueueY(x, y);

                Cell cell = SpawnCell(x, y, new Vector2(x, spawnY));
                cell.Type = (TileType)Random.Range(0, cell.dots.Length);
                cell.ChangePosition(x, y);
                spawned = true;
            }
        }

        return spawned;
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
