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
    public GameObject blurEffect;
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
    
   public void UpdateScreen(string screen)
    {
        switch (screen)
        {
            case "home":
                activeScreen = MainMenuActiveScreen.homeScreen;
                break;
            case "roomdecor":
                activeScreen = MainMenuActiveScreen.roomDecorScreen;
                break;
            case "roominventory":
                activeScreen = MainMenuActiveScreen.roomInventoryScreen;
                break;
            case "plushieinventory":
                activeScreen = MainMenuActiveScreen.plushieInventoryScreen;
                break;
        }
        ChangeActiveScreen();
    }
    public void ChangeActiveScreen()
    {
        switch(activeScreen)
        {
            case MainMenuActiveScreen.homeScreen:  
                homeScreen.SetActive(true);
                roomDecorScreen.SetActive(false);
                plushieInventoryScreen.SetActive(false);
                blurEffect.SetActive(true);
                break;
            case MainMenuActiveScreen.roomDecorScreen:
                homeScreen.SetActive(false);
                roomDecorScreen.SetActive(true);
                plushieInventoryScreen.SetActive(false);
                blurEffect.SetActive(false);
                break;
            case MainMenuActiveScreen.plushieInventoryScreen:
                homeScreen.SetActive(false);
                roomDecorScreen.SetActive(false);
                plushieInventoryScreen.SetActive(true);
                blurEffect.SetActive(true);
                break;
            case MainMenuActiveScreen.roomInventoryScreen:
                homeScreen.SetActive(false);
                //roomDecorScreen.SetActive(false);
                plushieInventoryScreen.SetActive(false);
                blurEffect.SetActive(false);
                break;
        }
    }
}
