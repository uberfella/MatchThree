using UnityEngine;

public class Cell : MonoBehaviour
{
    public TileType Type;
    public GameObject[] dots;
    public int x = -1;
    public int y = -1;
    public float fallSpeed = 8f;

    private const int RightMouseButton = 1;

    private Vector3 target;
    // Set while a teleporting cell is still on its way to the exit; it reappears at this point
    private Vector3? teleportEntry;
    private Vector3 finalTarget;
    private bool moving;

    public bool IsMoving => moving;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    #region Visuals

    private void Initialize()
    {
        if (dots == null || dots.Length == 0)
        {
            Debug.LogWarning($"No dots assigned for {name}");
            return;
        }

        // Type is assigned by the Board so it can avoid starting matches
        GameObject dot = Instantiate(dots[(int)Type], transform.position, Quaternion.identity, transform);
        dot.name = gameObject.name;

        // Keeps the cell hidden while it waits above the board
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    #endregion

    #region Movement

    public void ChangePosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        Vector3 destination = new Vector3(x, y, transform.position.z);

        // Not through the teleport yet: only the place it ends up changes
        if (teleportEntry.HasValue)
        {
            finalTarget = destination;
            return;
        }

        target = destination;
        moving = true;
    }

    // Slides out through exitPoint, then reappears at entryPoint and falls to (x, y)
    public void TeleportTo(int x, int y, Vector2 exitPoint, Vector2 entryPoint)
    {
        this.x = x;
        this.y = y;
        float z = transform.position.z;
        target = new Vector3(exitPoint.x, exitPoint.y, z);
        teleportEntry = new Vector3(entryPoint.x, entryPoint.y, z);
        finalTarget = new Vector3(x, y, z);
        moving = true;
    }

    private void MoveTowardsTarget()
    {
        if (!moving) return;

        transform.position = Vector3.MoveTowards(transform.position, target, fallSpeed * Time.deltaTime);
        if (transform.position != target) return;

        if (teleportEntry.HasValue)
        {
            transform.position = teleportEntry.Value;
            teleportEntry = null;
            target = finalTarget;
            return;
        }

        moving = false;
    }

    #endregion

    #region Input

    // Needs a Collider2D on the prefab
    void OnMouseDown()
    {
        Board board = GetComponentInParent<Board>();
        if (board != null)
        {
            // Removing a cell mid-fall would desync the grid from what's on screen
            if (board.IsBusy) return;
            board.RemoveCell(this);
        }

        Destroy(gameObject);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(RightMouseButton))
        {
            OnRightClick();
        }
    }

    private void OnRightClick()
    {
        Debug.Log($"Right-clicked on: {gameObject.name} {x}, {y}");
    }

    #endregion
}
