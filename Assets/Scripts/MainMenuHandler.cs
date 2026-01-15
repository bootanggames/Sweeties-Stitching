using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : Singleton<MainMenuHandler>
{
    [SerializeField] string SceneName;
    [SerializeField] TextMeshProUGUI coinText;
    public MainMenuActiveScreen activeScreen;
    public MainMenuActiveScreen previousScreen;
    public MainMenuScreens screens;
    [field:SerializeField] public LevelsInfoOnSelection levels {  get; private set; }

    public override void SingletonAwake()
    {
        base.SingletonAwake();
        LockUnLock();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }
    public override void Start()
    {
        Time.timeScale = 1;

        activeScreen = MainMenuActiveScreen.homeScreen;
        int c = PlayerPrefs.GetInt("Coins");
        coinText.text = c.ToString();
        int levelUp = PlayerPrefs.GetInt("LevelUp");
        int completed = PlayerPrefs.GetInt("PlushieCompleted");
        if (levelUp == 0)
        {
            if(completed == 1)
            {
                screens.disableControlsScreen.SetActive(true);
                StartCoroutine(levels.AnimateCurrentUnlockedPlushie());
                PlayerPrefs.SetInt("PlushieCompleted", 0);
            }
            else
            {
                levels.NextLevelPage();
            }
        }
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
        previousScreen = activeScreen;

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
            case "store":
                activeScreen = MainMenuActiveScreen.storeScreen;
                break;
        }
        ChangeActiveScreen();
    }
    public void ChangeActiveScreen()
    {
        switch(activeScreen)
        {
            case MainMenuActiveScreen.homeScreen:
                screens.homeScreenCanvas.SetActive(true);
                screens.homeScreen.SetActive(true);
                screens.roomDecorScreen.SetActive(false);
                screens.plushieInventoryScreen.SetActive(false);
                screens.blurEffect.SetActive(true);
                screens.storeScreen.SetActive(false);

                int c = PlayerPrefs.GetInt("Coins");
                coinText.text = c.ToString();
                break;
            case MainMenuActiveScreen.roomDecorScreen:
                screens.homeScreen.SetActive(false);
                screens.roomDecorScreen.SetActive(true);
                screens.plushieInventoryScreen.SetActive(false);
                screens.blurEffect.SetActive(false);
                screens.storeScreen.SetActive(false);

                break;
            case MainMenuActiveScreen.plushieInventoryScreen:
                screens.homeScreen.SetActive(false);
                screens.roomDecorScreen.SetActive(false);
                screens.plushieInventoryScreen.SetActive(true);
                screens.blurEffect.SetActive(true);
                screens.storeScreen.SetActive(false);

                break;
            case MainMenuActiveScreen.roomInventoryScreen:
                screens.homeScreen.SetActive(false);
                screens.plushieInventoryScreen.SetActive(false);
                screens.blurEffect.SetActive(false);
                screens.storeScreen.SetActive(false);

                break;
            case MainMenuActiveScreen.storeScreen:
                screens.homeScreen.SetActive(false);
                screens.blurEffect.SetActive(true);
                screens.plushieInventoryScreen.SetActive(false);
                screens.roomDecorScreen.SetActive(false);
                screens.storeScreen.SetActive(true);
                break;
        }
    }

    void LockUnLock()
    {
        PlayerPrefs.SetInt("Level_" + 0 + "Plushie_" + 0, 1);

        for (int i = 0; i < levels.levelPage.Count; i++)
        {
            for (int j = 0; j < levels.levelPage[i].levelDetail.Count; j++)
            {
                LevelDetail levelD = levels.levelPage[i].levelDetail[j].levelObject.GetComponent<LevelDetail>();
                levelD.CheckLevelLockeUnlocked(i, j);
            }
        }
    }
}
