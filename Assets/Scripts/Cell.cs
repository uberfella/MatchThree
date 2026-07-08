using UnityEngine;

public class Cell : MonoBehaviour
{
    public TileType Type;
    public GameObject[] dots;
    void Start()
    {
        Initialize();
    }

    void Update()
    {

    }

    private void Initialize()
    {
        int dotToUse = Random.Range(0, dots.Length);
        Type = (TileType)dotToUse;
        GameObject dot = Instantiate(dots[dotToUse], transform.position, Quaternion.identity);
        Debug.Log("used dot " + dotToUse);
        Debug.Log("used type " + Type);
        dot.transform.parent = this.transform;
        dot.name = this.gameObject.name;
    }
}
