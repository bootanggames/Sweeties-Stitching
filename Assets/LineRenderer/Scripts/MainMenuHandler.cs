using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] string SceneName;
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
