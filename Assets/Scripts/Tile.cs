using UnityEngine;

public class Tile : MonoBehaviour
{
    //public TileType type;
    //public bool isMatched;

    public int X;
    public int Y;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(TileType type)
    {
        spriteRenderer.sprite = sprites[(int)type];
    }
}
