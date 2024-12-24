using UnityEngine;
using UnityEngine.EventSystems;

public class DetectClick : MonoBehaviour
{
    //private StartGame startGame;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color glowColor = Color.cyan;
    private float interval = 0.33f;
    private float timer = 0f;
    public bool tileIsChosen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Store the original color
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (tileIsChosen)
        {
            //spriteRenderer.color = glowColor;
            //spriteRenderer.color = originalColor;
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                if (spriteRenderer.color == originalColor)
                {
                    spriteRenderer.color = glowColor;
                }
                else
                {
                    spriteRenderer.color = originalColor;
                }
                timer = 0f;
            }
        }

    }

    public void Select()
    {
        tileIsChosen = true;
        //GetComponent<SpriteRenderer>().color = Color.yellow;
        //Debug.Log($"{gameObject.name} is now selected.");
    }

    public void Deselect()
    {
        tileIsChosen = false;
        GetComponent<SpriteRenderer>().color = originalColor;
        //Debug.Log($"{gameObject.name} is now deselected.");
    }



}

