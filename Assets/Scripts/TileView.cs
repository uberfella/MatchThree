using System;
using UnityEngine;

public class TileView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    public int X;
    public int Y;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(TileType type)
    {
        spriteRenderer.sprite = sprites[(int)type];
    }
}
