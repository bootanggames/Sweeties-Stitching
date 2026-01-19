using System.Collections.Generic;
using UnityEngine;

public class JackpotHandler : MonoBehaviour, IJackpotHandler
{
    [field: SerializeField] public JackpotData jackpotData {  get; private set; }
    [field: SerializeField] public List<ItemsMetaData> earnedItems { get; private set; }

    GameObject jackpotScreen = null;
    IRoomdecorStore _roomdecorScreen;
    private void OnEnable()
    {
        RegisterService();
    }
    private void Start()
    {
        _roomdecorScreen = ServiceLocator.GetService<IRoomdecorStore>();
        earnedItems = new List<ItemsMetaData>();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void ShowJackPotScreen()
    {
        Invoke(nameof(ShowWithDelay), 0.5f);
    }

  
    void ShowWithDelay()
    {
        jackpotScreen = Instantiate(jackpotData.jackpotPrefab);
        jackpotScreen.SetActive(true);
        jackpotData.mainCamera.SetActive(false);
        if (MainMenuHandler.instance)
        {
            MainMenuHandler.instance.screens.homeScreenCanvas.SetActive(false);
            MainMenuHandler.instance.screens.mainMenuBg.SetActive(false);
        }
        jackpotData.roomDecorCanvas.SetActive(false);
        jackpotData.plushieInventoryCanvas.SetActive(false);
        //jackpotData.roomBg.SetActive(false);
        CancelInvoke(nameof(ShowWithDelay));
    }
    public void CloseJackpotScreen()
    {
        jackpotData.mainCamera.SetActive(true);
        if (MainMenuHandler.instance)
        {
            MainMenuHandler.instance.screens.homeScreenCanvas.SetActive(true);
            MainMenuHandler.instance.screens.mainMenuBg.SetActive(true);
        }
        //jackpotData.roomBg.SetActive(true);
        if (jackpotScreen != null)
            Destroy(jackpotScreen);
    }
    public void CloseRewardScreen()
    {
        jackpotData.mainCamera.SetActive(true);
        _roomdecorScreen.MyRoomButton();
        if (MainMenuHandler.instance)
        {
            MainMenuHandler.instance.screens.homeScreenCanvas.SetActive(true);
            MainMenuHandler.instance.screens.homeScreen.SetActive(false);
            MainMenuHandler.instance.screens.mainMenuBg.SetActive(true);
        }
        //jackpotData.roomBg.SetActive(true);

        if (jackpotScreen != null)
            Destroy(jackpotScreen);
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IJackpotHandler>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IJackpotHandler>(this);
    }
}
