using System.Collections.Generic;
using UnityEngine;

public class HomeScreenTutorial : MonoBehaviour
{
    bool startTutorial = false;
    [SerializeField] GameObject tutorialPanel;

    [SerializeField] List<TutorialScreenWithType> tutorialScreen;
    [SerializeField] GameObject finishedTutorialScreen;
    [SerializeField] GameObject skipBtn;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject homeBtn;
    [SerializeField] GameObject sewBtnHomeMenu;
    [SerializeField] int mainScreenIndex = 0;
    [SerializeField] int screenIndex = 0;
    [SerializeField] int subScreenIndex = 0;
    [SerializeField] bool gameplayScreen;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("TutorialFinished") || PlayerPrefs.GetInt("TutorialFinished") == 0)
            startTutorial = true;
        else
            startTutorial = false;
        if (gameplayScreen) return;
        if (startTutorial) StartTutorial();
    }
    public void StartTutorial()
    {
        if(sewBtnHomeMenu)
            sewBtnHomeMenu.SetActive(false);
        nextBtn.SetActive(true);
        tutorialPanel.SetActive(true);
        skipBtn.SetActive(true);
        finishedTutorialScreen.SetActive(false);
        for (int i = 0; i < tutorialScreen.Count; i++)
        {
            if (PlayerPrefs.GetInt(tutorialScreen[i].screenName) == 0)
            {
                mainScreenIndex = i;
                break;
            }
        }

        tutorialScreen[mainScreenIndex].screenParent.SetActive(true);
        for (int j = 0; j < tutorialScreen[mainScreenIndex].screens.Count; j++)
        {
            if (PlayerPrefs.GetInt(tutorialScreen[mainScreenIndex].screens[screenIndex].screen.name + "_" + screenIndex) == 0)
            {
                screenIndex = j;
                break;
            }
        }
        tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(true);
        if(gameplayScreen)
            nextBtn.SetActive(false);
    }
    public void NextScreenButtonForSubScreenTutorial()
    {
        nextBtn.SetActive(true);
        tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(false);
        PlayerPrefs.SetInt(tutorialScreen[mainScreenIndex].screens[screenIndex].screen.name + "_" + screenIndex, 1);
        tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens[subScreenIndex].SetActive(true);
    }
    public void SubScreenNextButton()
    {
        tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens[subScreenIndex].SetActive(false);
        subScreenIndex++; 
        if (subScreenIndex >= tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens.Count)
        {
            foreach (GameObject g in tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens)
            {
                g.SetActive(false);
            }
            subScreenIndex = 0;
            screenIndex++;
            CheckScreenActivity();

            if (tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens.Count > 0)
            {
                SubScreenActivation();
            }
            if (MainMenuHandler.instance)
            {
                MainMenuHandler.instance.screens.homeScreen.SetActive(true);
                MainMenuHandler.instance.screens.homeScreenCanvas.SetActive(true);
            }
        }
        else
        {
            tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens[subScreenIndex].SetActive(true);
        }

    }
    
    public void CheckFinishedStatus()
    {
        if(tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens.Count > 0 && subScreenIndex >= 0)
        {
            SubScreenNextButton();
            return;
        }
        if (tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens.Count > 0)
        {
            SubScreenActivation();
        }
        else
        {
            PlayerPrefs.SetInt(tutorialScreen[mainScreenIndex].screens[screenIndex].screen.name + "_" + screenIndex, 1);
            tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(false);
            screenIndex++;
            if(screenIndex < tutorialScreen[mainScreenIndex].screens.Count)
            {
                if (tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens.Count > 0)
                    SubScreenActivation();
            }

            CheckScreenActivity();
        }
       
    }
    void SubScreenActivation()
    {
        nextBtn.SetActive(false);
        if (!tutorialScreen[mainScreenIndex].screens[screenIndex].screen.activeSelf)
            tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(true);
    }
    void CheckScreenActivity()
    {
        if (screenIndex >= tutorialScreen[mainScreenIndex].screens.Count)
        {
            PlayerPrefs.SetInt(tutorialScreen[mainScreenIndex].screenName, 1);
            screenIndex = 0;
            tutorialScreen[mainScreenIndex].screenParent.SetActive(false);
            mainScreenIndex++;
            if (mainScreenIndex >= tutorialScreen.Count)
            {
                mainScreenIndex = 0;
                nextBtn.SetActive(false);
                skipBtn.SetActive(false);
                homeBtn.SetActive(true);

                finishedTutorialScreen.SetActive(true);
                HomeMenuActivation();
            }
            else
            {
                tutorialScreen[mainScreenIndex].screenParent.SetActive(true);
                tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(true);
            }
        }
        else
        {
            if (gameplayScreen)
            {
                if(screenIndex >= 2)
                {
                    LevelsHandler.instance.currentLevelMeta.SetAllPointsOfFirstStitchPartActive();
                }
            }
            tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(true);

        }
    }
    public void SkipButton()
    {
        screenIndex = 0;
        mainScreenIndex = 0;
        subScreenIndex = 0;

        PlayerPrefs.SetInt("TutorialFinished", 1);
        foreach(TutorialScreenWithType sp in tutorialScreen)
        {
            sp.screenParent.SetActive(false);
            foreach(TutorialScreens g in sp.screens)
            {
                g.screen.SetActive(false);
                foreach(GameObject sg in g.subScreens)
                {
                    sg.SetActive(false);
                }
            }
        }
        HomeMenuActivation();
        if(sewBtnHomeMenu)
            sewBtnHomeMenu.SetActive(true);

        if (gameplayScreen)
            PlayerPrefs.SetInt("GameplayTutorialFinished", 1);
        tutorialPanel.SetActive(false);

    }
    void HomeMenuActivation()
    {
        if (MainMenuHandler.instance)
        {
            MainMenuHandler.instance.screens.homeScreen.SetActive(true);
            MainMenuHandler.instance.screens.homeScreenCanvas.SetActive(true);
            MainMenuHandler.instance.screens.roomDecorScreen.SetActive(false);
            MainMenuHandler.instance.screens.plushieInventoryScreen.SetActive(false);
        }
    }
}
