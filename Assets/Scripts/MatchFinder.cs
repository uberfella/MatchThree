using System.Collections.Generic;
using UnityEngine;

public static class MatchFinder
{
    public static bool[,] FindMatches(Cell[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        bool[,] matched = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell == null || cell.isPowerup) continue;

                if (x + 2 < width && IsSameType(cell, grid[x + 1, y]) && IsSameType(cell, grid[x + 2, y]))
                {
                    matched[x, y] = matched[x + 1, y] = matched[x + 2, y] = true;
                }

                if (y + 2 < height && IsSameType(cell, grid[x, y + 1]) && IsSameType(cell, grid[x, y + 2]))
                {
                    matched[x, y] = matched[x, y + 1] = matched[x, y + 2] = true;
                }
            }
        }

        return matched;
    }

    // For every line of exactly four matching cells, returns the second cell of the line (where a powerup appears)
    public static List<Vector2Int> FindFourInARow(Cell[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        List<Vector2Int> spots = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell == null || cell.isPowerup) continue;

                // A four-line that is part of a bomb pattern makes a bomb instead of a rocket
                if (!(x > 0 && IsSameType(cell, grid[x - 1, y])) && RunLength(grid, x, y, 1, 0) == 4
                    && !RunHasJunction(grid, x, y, 1, 0, 4))
                    spots.Add(new Vector2Int(x + 1, y));

                if (!(y > 0 && IsSameType(cell, grid[x, y - 1])) && RunLength(grid, x, y, 0, 1) == 4
                    && !RunHasJunction(grid, x, y, 0, 1, 4))
                    spots.Add(new Vector2Int(x, y + 1));
            }
        }

        return spots;
    }

    // Color booster spots: the middle of a straight line of 5+
    public static List<Vector2Int> FindColorBoosters(Cell[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        List<Vector2Int> spots = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell == null || cell.isPowerup) continue;

                if (!(x > 0 && IsSameType(cell, grid[x - 1, y])) && RunLength(grid, x, y, 1, 0) >= 5)
                    spots.Add(new Vector2Int(x + 2, y));

                if (!(y > 0 && IsSameType(cell, grid[x, y - 1])) && RunLength(grid, x, y, 0, 1) >= 5)
                    spots.Add(new Vector2Int(x, y + 2));
            }
        }

        return spots;
    }

    // Bomb spots: the crossing cell of an L, T or + (a horizontal and a vertical line of 3+ sharing a cell)
    public static List<Vector2Int> FindBombs(Cell[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        List<Vector2Int> spots = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell == null || cell.isPowerup) continue;

                if (IsJunction(grid, x, y)) spots.Add(new Vector2Int(x, y));
            }
        }

        return spots;
    }

    private static bool IsJunction(Cell[,] grid, int x, int y)
    {
        return LineThrough(grid, x, y, 1, 0) >= 3 && LineThrough(grid, x, y, 0, 1) >= 3;
    }

    // Length of the same-type line passing through the cell along (dx, dy)
    private static int LineThrough(Cell[,] grid, int x, int y, int dx, int dy)
    {
        Cell cell = grid[x, y];
        int length = 1;
        for (int i = 1; InBounds(grid, x + dx * i, y + dy * i) && IsSameType(cell, grid[x + dx * i, y + dy * i]); i++) length++;
        for (int i = 1; InBounds(grid, x - dx * i, y - dy * i) && IsSameType(cell, grid[x - dx * i, y - dy * i]); i++) length++;
        return length;
    }

    private static bool RunHasJunction(Cell[,] grid, int x, int y, int dx, int dy, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (IsJunction(grid, x + dx * i, y + dy * i)) return true;
        }
        return false;
    }

    private static bool InBounds(Cell[,] grid, int x, int y)
    {
        return x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1);
    }

    private static int RunLength(Cell[,] grid, int x, int y, int dx, int dy)
    {
        Cell start = grid[x, y];
        int length = 1;

        while (x + dx * length < grid.GetLength(0) && y + dy * length < grid.GetLength(1)
            && IsSameType(start, grid[x + dx * length, y + dy * length]))
        {
            length++;
        }

        return length;
    }

    private static bool IsSameType(Cell a, Cell b)
    {
        return b != null && !a.isPowerup && !b.isPowerup && a.Type == b.Type;
    }
}
