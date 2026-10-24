using UnityEngine;

// New cells spawn above the grid; masking hides them until they slide into view
public static class PlayAreaMask
{
    public static void Create(Transform parent, int width, int height)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        // A mask needs a sprite; a 1x1 unit one scales cleanly to any board size
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

        GameObject maskObject = new GameObject("PlayAreaMask");
        maskObject.transform.SetParent(parent, false);
        // Cells are placed in world space at integer coordinates (the Board object itself sits elsewhere),
        // so the grid spans -0.5 .. size-0.5 in world units
        maskObject.transform.position = new Vector3((width - 1) / 2f, (height - 1) / 2f, 0f);
        maskObject.transform.localScale = new Vector3(width, height, 1f);

        SpriteMask mask = maskObject.AddComponent<SpriteMask>();
        mask.sprite = sprite;
    }
}
