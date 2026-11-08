using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName = "OmniLevel";

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(sceneName));
    }
}
