using System.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public TileType Type;
    // A powerup never matches; tapping it triggers it instead
    public bool isPowerup;
    public GameObject[] dots;
    public int x = -1;
    public int y = -1;
    public float fallSpeed = 8f;

    private const int RightMouseButton = 1;
    // World units the cursor must travel from the press point to count as a drag
    private const float DragThreshold = 0.4f;
    private const float HighlightSeconds = 0.15f;
    private const float HighlightStrength = 0.6f;

    private bool pressed;
    private bool dragHandled;
    private Vector2 pressPoint;
    private Coroutine highlight;

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
        if (!isPowerup)
        {
            if (dots == null || dots.Length == 0)
            {
                Debug.LogWarning($"No dots assigned for {name}");
                return;
            }

            // Type is assigned by the Board so it can avoid starting matches
            GameObject dot = Instantiate(dots[(int)Type], transform.position, Quaternion.identity, transform);
            dot.name = gameObject.name;
        }

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

        if (IsDeleteModifierHeld())
        {
            RemoveSelf(board);
            return;
        }

        // Moving a cell mid-fall would desync the grid from what's on screen
        if (board != null && board.IsBusy) return;

        pressed = true;
        dragHandled = false;
        pressPoint = GetMouseWorldPoint();
    }

    // Keeps firing while the button is held, even after the cursor leaves the collider
    void OnMouseDrag()
    {
        if (!pressed || dragHandled) return;

        Vector2 delta = GetMouseWorldPoint() - pressPoint;
        if (delta.magnitude < DragThreshold) return;

        // One swap attempt per press, along whichever axis the cursor travelled further
        dragHandled = true;
        Vector2Int direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
            ? new Vector2Int((int)Mathf.Sign(delta.x), 0)
            : new Vector2Int(0, (int)Mathf.Sign(delta.y));

        Board board = GetComponentInParent<Board>();
        if (board != null) board.TrySwap(this, direction);
    }

    void OnMouseUp()
    {
        // A press that never turned into a drag is a tap
        if (pressed && !dragHandled)
        {
            Board board = GetComponentInParent<Board>();
            if (isPowerup && board != null) board.ActivatePowerup(this);
            else Highlight();
        }
        pressed = false;
    }

    private static bool IsDeleteModifierHeld()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
            || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    }

    private void RemoveSelf(Board board)
    {
        if (board != null)
        {
            if (board.IsBusy) return;
            board.RemoveCell(this);
        }

        Destroy(gameObject);
    }

    private Vector2 GetMouseWorldPoint()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Highlight()
    {
        if (highlight == null) highlight = StartCoroutine(HighlightRoutine());
    }

    private IEnumerator HighlightRoutine()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Color[] originals = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originals[i] = renderers[i].color;
            renderers[i].color = Color.Lerp(originals[i], Color.white, HighlightStrength);
        }

        yield return new WaitForSeconds(HighlightSeconds);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = originals[i];
        }
        highlight = null;
    }

    void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
        {
            Board board = GetComponentInParent<Board>();
            if (board != null) board.ConvertToRocket(this);
            return;
        }

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
