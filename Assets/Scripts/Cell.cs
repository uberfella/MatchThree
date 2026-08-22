using UnityEngine;
using UnityEngine.InputSystem;

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

    public void ChangePosition(int x, int y) 
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector3(x, y, transform.position.z);

        Debug.Log($"moving {gameObject.name} to {x}, {y}");
    }

    private void OnMouseOver()
    {
        // 1 represents the right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            OnRightClick();
        }
    }

    private void OnRightClick()
    {
        Debug.Log($"Right-clicked on: {gameObject.name} {x}, {y}");
        // Add your custom logic here
        //ChangePosition(1, 1);
    }
}
