using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : Singleton<MainMenuHandler>
{
    [SerializeField] string SceneName;
    [SerializeField] TextMeshProUGUI coinText;
    public MainMenuActiveScreen activeScreen;

    public GameObject roomDecorScreen;
    public GameObject homeScreen;
    public GameObject plushieInventoryScreen;
    public GameObject mainMenuBg;
    public override void SingletonAwake()
    {
        base.SingletonAwake();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }
    public override void Start()
    {
        activeScreen = MainMenuActiveScreen.homeScreen;
        int c = PlayerPrefs.GetInt("Coins");
        coinText.text = c.ToString();
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void PlushieInventoryScreen()
    {
       activeScreen = MainMenuActiveScreen.plushieInventoryScreen;
    }
   public void RoomDecorScreen()
    {
      activeScreen = MainMenuActiveScreen.roomDecorScreen;
    }
}
