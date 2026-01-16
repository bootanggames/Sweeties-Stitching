using System.Collections.Generic;
using UnityEngine;

public class HomeScreenTutorial : MonoBehaviour
{
    bool startTutorial = false;
    [SerializeField] GameObject tutorialPanel;

    [SerializeField] List<TutorialScreenWithType> tutorialScreen;
    [SerializeField] GameObject skipBtn;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject homeBtn;
    [SerializeField] int mainScreenIndex = 0;
    [SerializeField] int screenIndex = 0;
    [SerializeField] int subScreenIndex = 0;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("TutorialFinished") || PlayerPrefs.GetInt("TutorialFinished") == 0)
            startTutorial = true;
        else
            startTutorial = false;

        if (startTutorial)
        {
            tutorialPanel.SetActive(true);
            for (int i=0;i<tutorialScreen.Count;i++)
            {
                if (PlayerPrefs.GetInt(tutorialScreen[i].screenName) == 0)
                {
                    mainScreenIndex = i;
                    break;
                }
            }
           
            tutorialScreen[mainScreenIndex].screenParent.SetActive(true);
            for(int j=0;j< tutorialScreen[mainScreenIndex].screens.Count; j++)
            {
                if(PlayerPrefs.GetInt(tutorialScreen[mainScreenIndex].screens[screenIndex].screen.name + "_" + screenIndex) == 0)
                {
                    screenIndex = j;
                    break;
                }
            }
            tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(true);
        }
    }

    public void NextScreenButtonForSubScreenTutorial()
    {
        tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(false);
        PlayerPrefs.SetInt(tutorialScreen[mainScreenIndex].screens[screenIndex].screen.name + "_" + screenIndex, 1);
        tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens[subScreenIndex].SetActive(true);
    }
    public void SubScreenNextButton()
    {
        subScreenIndex++;
        if (subScreenIndex >= tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens.Count)
        {
            subScreenIndex = 0;
            screenIndex++;
            foreach(GameObject g in tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens)
            {
                g.SetActive(false);
            }
            CheckFinishedStatus();
        }
    }
    public void CheckFinishedStatus()
    {
        if(subScreenIndex > 0)
        {
            SubScreenNextButton();
            return;
        }
        if(tutorialScreen[mainScreenIndex].screens[screenIndex].subScreens.Count > 0)
        {
            nextBtn.SetActive(false);
            NextScreenButtonForSubScreenTutorial();
        }
        else
        {
            PlayerPrefs.SetInt(tutorialScreen[mainScreenIndex].screens[screenIndex].screen.name + "_" + screenIndex, 1);
            tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(false);
            screenIndex++;
            if (screenIndex >= tutorialScreen[mainScreenIndex].screens.Count)
            {
                PlayerPrefs.SetInt(tutorialScreen[mainScreenIndex].screenName, 1);
                screenIndex = 0;
                tutorialScreen[mainScreenIndex].screenParent.SetActive(false);
                mainScreenIndex++;
                if (mainScreenIndex >= tutorialScreen.Count)
                {
                    SkipButton();
                }
                else
                {
                    tutorialScreen[mainScreenIndex].screenParent.SetActive(true);
                    tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(true);
                }
            }
            else
            {
                tutorialScreen[mainScreenIndex].screens[screenIndex].screen.SetActive(true);

            }
        }
       
    }

    public void SkipButton()
    {
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
        tutorialPanel.SetActive(false);
    }
}
