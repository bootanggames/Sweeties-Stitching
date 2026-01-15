using TMPro;
using TS.PageSlider;
using UnityEngine;

public class PlushiesInventory : MonoBehaviour,IPlushieInventory
{
    [field:SerializeField] public PageContainer[] plushies {  get; private set; }
    [SerializeField] TextMeshProUGUI coinUi;
    [field:SerializeField] public TextMeshProUGUI totalPlushies {  get; private set; }
    [SerializeField] PageScroller pageScroller;
    [SerializeField] PageSlider pageSlider;
    [field: SerializeField] public int noOfPlushieEnabled {  get; private set; }
    private void OnEnable()
    {
        int c = PlayerPrefs.GetInt("Coins");
        coinUi.text = c.ToString();
        RegisterService();
        if (AudiosSourceContainer.instance)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.homeScreen);
            SoundManager.instance.PlaySound(AudiosSourceContainer.instance.plushieInventoryScreen, SoundManager.instance.audioClips.plushieInventoryScreenBgSound, true, false, 1.0f, true);
        }

    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void StopPlushieScreenSound()
    {
        if (AudiosSourceContainer.instance)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.plushieInventoryScreen);

        }
    }
    public void BackButton()
    {
        if (AudiosSourceContainer.instance)
        {
            StopPlushieScreenSound();
            if (AudiosSourceContainer.instance.homeScreen)
                SoundManager.instance.PlaySound(AudiosSourceContainer.instance.homeScreen, SoundManager.instance.audioClips.bgMusic, true, false, 1.0f, true);
        }

    }
    public void BackButtonScreenActivation()
    {
        if (MainMenuHandler.instance)
        {
            if (MainMenuHandler.instance.previousScreen.Equals(MainMenuActiveScreen.roomDecorScreen))
            {
                //MainMenuHandler.instance.mainMenuBg.SetActive(false);
                MainMenuHandler.instance.UpdateScreen("roomdecor");
                MainMenuHandler.instance.screens.roomDecorScreen.SetActive(true);
                MainMenuHandler.instance.screens.plushieInventoryScreen.SetActive(false);
            }
            else
            {
                MainMenuHandler.instance.UpdateScreen("home");
                MainMenuHandler.instance.screens.homeScreen.SetActive(true);
                MainMenuHandler.instance.screens.plushieInventoryScreen.SetActive(false);

            }
        }
    }
    public void NoPlushieIncrement(int c)
    {
        noOfPlushieEnabled = c;
        totalPlushies.text = noOfPlushieEnabled.ToString();
    }
    public void NextPage()
    {
        if (pageScroller != null)
        {
            var page = pageScroller._currentPage;
            page++;
            if(page < pageSlider._pages.Count)
                pageScroller.ScrollToPage(page);
            else
                page = pageSlider._pages.Count - 1;
        }
    }
    public void PrevPage()
    {
        if (pageScroller != null)
        {
            var page = pageScroller._currentPage;
            page--;
            if (page >= 0)
                pageScroller.ScrollToPage(page);
            else
                page = 0;
        }
    }

    public void RegisterService()
    {
        ServiceLocator.RegisterService<IPlushieInventory>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPlushieInventory>(this);
    }
    public void Store()
    {
        if (GameHandler.instance)
        {
            ICanvasUIManager uimanager = ServiceLocator.GetService<ICanvasUIManager>();
            if(uimanager != null)
            {

                uimanager.screens.plushiesInventoryMainObject.SetActive(false);
                uimanager.screens.roomDecorScreen.SetActive(false);

                GameHandler.instance.SwitchGameState(GameStates.StoreScreen);
                uimanager.screens.storeScreen.SetActive(true);
            }
        }
    }
}
