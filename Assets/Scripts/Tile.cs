using UnityEngine;

public class Tile : MonoBehaviour, IDestroyable
{
    //public TileType type;
    //public bool isMatched;

    public int X;
    public int Y;

    private TilesManager tilesManager;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    public void Init(int x, int y, TilesManager manager)
    {
        X = x;
        Y = y;
        tilesManager = manager;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(TileType type)
    {
        spriteRenderer.sprite = sprites[(int)type];
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        tilesManager.OnTileClicked(X, Y, gameObject);
    }
}
