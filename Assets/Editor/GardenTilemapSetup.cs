using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GardenTilemapSetup
{
    // name, local Y offset (only applied when created), order in layer
    static readonly (string name, float y, int order)[] Layers =
    {
        ("Ground_lowered", -0.94f, -1),
        ("Tilemap", 0f, 0),
        ("Slightly_higher_Tilemap", 0.2f, 1),
        ("Raised_areas", 0.94f, 2),
    };

    [MenuItem("Tools/Garden/Setup Tilemaps")]
    static void Setup()
    {
        var grid = Object.FindFirstObjectByType<Grid>();
        if (grid == null)
        {
            Debug.LogError("No Grid in the open scene.");
            return;
        }

        foreach (var (name, y, order) in Layers)
        {
            var child = grid.transform.Find(name);
            if (child == null)
            {
                var go = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
                Undo.RegisterCreatedObjectUndo(go, "Create " + name);
                go.transform.SetParent(grid.transform, false);
                go.transform.localPosition = new Vector3(0f, y, 0f);
                child = go.transform;
            }

            var tr = child.GetComponent<TilemapRenderer>();
            Undo.RecordObject(tr, "Setup " + name);
            tr.mode = TilemapRenderer.Mode.Individual;
            tr.sortOrder = TilemapRenderer.SortOrder.TopRight;
            tr.sortingOrder = order;
            child.localRotation = Quaternion.identity;
            EditorUtility.SetDirty(tr);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(grid.gameObject.scene);
        Debug.Log("Garden tilemaps set up.");
    }
}
