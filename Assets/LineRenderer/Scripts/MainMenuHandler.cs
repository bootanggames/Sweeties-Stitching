using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] string SceneName;
    [SerializeField] TextMeshProUGUI coinText;
    private void Start()
    {
        int c = PlayerPrefs.GetInt("Coins");
        coinText.text = c.ToString();
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
