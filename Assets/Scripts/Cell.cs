using UnityEngine;

public class Cell : MonoBehaviour
{
    public TileType Type;
    public GameObject[] dots;
    public int x = -1;
    public int y = -1;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (dots == null || dots.Length == 0)
        {
            Debug.LogWarning($"No dots assigned for {name}");
            return;
        }

        int dotToUse = Random.Range(0, dots.Length);
        Type = (TileType)dotToUse;
        GameObject dot = Instantiate(dots[dotToUse], transform.position, Quaternion.identity, transform);
        dot.name = this.gameObject.name;
    }

    // Invoked when the object is clicked (requires Collider2D + Physics2D raycasts)
    void OnMouseDown()
    {
        // Notify board to clear its reference for this cell
        Board board = GetComponentInParent<Board>();
        if (board != null)
        {
            board.RemoveCell(this);
        }

        // Destroy this cell GameObject
        Destroy(gameObject);
    }
}
