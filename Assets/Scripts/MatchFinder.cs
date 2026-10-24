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
                if (cell == null) continue;

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

    private static bool IsSameType(Cell a, Cell b)
    {
        return b != null && a.Type == b.Type;
    }
}
