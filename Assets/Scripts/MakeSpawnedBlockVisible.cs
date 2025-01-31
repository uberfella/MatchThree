using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class MakeSpawnedBlockVisible : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float visibilityThresholdY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Cache the SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; 
        }

        // Get the row threshold dynamically
        visibilityThresholdY = GridManager.rows - 0.5f;
        //Debug.Log("visibilityThresholdY = " + visibilityThresholdY);
        //Debug.Log("transform.position.y = " + transform.position.y);

        float spawnPosY = GridManager.rows;
        //Vector2 spawnPositionAdditional = new Vector2(spawnPosX, spawnPosY);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= visibilityThresholdY) 
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
        }
    }
}
