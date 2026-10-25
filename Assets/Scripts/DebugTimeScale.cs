using UnityEngine;

// Debug helper: '-' / '[' slow down, '=' / ']' speed up, '0' resets the time scale.
// Creates itself on play, so it doesn't need to be added to the scene
public class DebugTimeScale : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private static readonly float[] Scales = { 0.05f, 0.1f, 0.25f, 0.5f, 1f };
    private const int NormalIndex = 4;

    private int index = NormalIndex;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        if (FindFirstObjectByType<DebugTimeScale>() != null) return;

        new GameObject("DebugTimeScale").AddComponent<DebugTimeScale>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)
            || Input.GetKeyDown(KeyCode.LeftBracket))
            SetIndex(index - 1);

        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)
            || Input.GetKeyDown(KeyCode.RightBracket))
            SetIndex(index + 1);

        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
            SetIndex(NormalIndex);
    }

    private void SetIndex(int newIndex)
    {
        int previous = index;
        index = Mathf.Clamp(newIndex, 0, Scales.Length - 1);
        Time.timeScale = Scales[index];

        string direction = index < previous ? "down" : index > previous ? "up" : "unchanged";
        Debug.Log($"Time scale {direction}: {Time.timeScale}");
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }
#endif
}
