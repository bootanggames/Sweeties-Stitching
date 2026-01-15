using TMPro;
using UnityEngine;

public class StoreItemsInventory : MonoBehaviour
{
    [SerializeField] GameObject useButtonObj;
    [SerializeField] GameObject buyButtonObj;
    [SerializeField] TextMeshProUGUI coinsText;
    DecorItemsInventory itemsInventory;
    int coins = 0;
    ICanvasUIManager uimanger;
    private void Start()
    {
        itemsInventory = this.GetComponent<DecorItemsInventory>();
        UpdateCoins();
    }
    public void UpdateCoins()
    {
        coins = PlayerPrefs.GetInt("Coins");
        coinsText.text = coins.ToString();
    }
    public void BuyItem()
    {
        int _iPrice = 0;
        foreach (DecoreItemStoreButton b in itemsInventory.ButtonsList())
        {
            UIWidget uIWidget = b.GetComponent<UIWidget>();
            if (itemsInventory.clickedItemContext.ID.Equals(uIWidget.GetContextID()))
            {
                _iPrice = uIWidget.GetItemPrice();
                if (coins >= _iPrice)
                {
                    coins -= _iPrice;
                    coinsText.text = coins.ToString();
                    PlayerPrefs.SetInt("Coins", coins);
                    uIWidget._itemInfo.Unlock();
                    buyButtonObj.SetActive(false);
                    useButtonObj?.SetActive(true);
                    break;
                }
            }
        }

    }

    public void ButtonsActivity(UIContext context)
    {
        foreach (DecoreItemStoreButton b in itemsInventory.ButtonsList())
        {
            UIWidget uIWidget = b.GetComponent<UIWidget>();

            if (uIWidget.GetContextID().Equals(context.ID))
            {
                ItemInfo info = uIWidget.gameObject.GetComponent<ItemInfo>();
                if (info.status.Equals(ItemStatus.locked))
                {
                    buyButtonObj?.SetActive(true);
                    useButtonObj?.SetActive(false);
                }
                else
                {
                    buyButtonObj?.SetActive(false);
                    useButtonObj?.SetActive(true);
                }
            }
        }
            
    }

    public void GoToHome()
    {
        if (MainMenuHandler.instance)
            MainMenuHandler.instance.UpdateScreen("home");
    }

    public void Back()
    {
        if (MainMenuHandler.instance)
        {
            switch (MainMenuHandler.instance.previousScreen)
            {
                case MainMenuActiveScreen.homeScreen:
                    BackToScreen("home");
                    break;
                case MainMenuActiveScreen.plushieInventoryScreen:
                    BackToScreen("plushieinventory");
                    break;
                case MainMenuActiveScreen.roomDecorScreen:
                    BackToScreen("roomdecor");
                    break;
                case MainMenuActiveScreen.roomInventoryScreen:
                    BackToScreen("roominventory");
                    break;
            }
        }
        if (GameHandler.instance)
        {
            if (uimanger == null)
                uimanger = ServiceLocator.GetService<ICanvasUIManager>();
            uimanger.screens.storeBg.SetActive(false);

            switch (GameHandler.instance.previousScreen)
            {
                case GameStates.PlushieInventorySceen:
                    uimanger.screens.plushiesInventoryMainObject.SetActive(true);
                    uimanger.screens.storeScreen.SetActive(false);
                    GameHandler.instance.SwitchGameState(GameStates.PlushieInventorySceen);
                    break;
                case GameStates.RoomDecorScreen:
                    uimanger.screens.storeScreen.SetActive(false);
                    uimanger.screens.plushiesInventoryMainObject.SetActive(true);
                    uimanger.screens.roomDecorScreen.SetActive(true);
                    GameHandler.instance.SwitchGameState(GameStates.RoomDecorScreen);

                    break;
            }
        }
    }
    void BackToScreen(string screen)
    {
        if (MainMenuHandler.instance)
            MainMenuHandler.instance.UpdateScreen(screen);
    }
}
