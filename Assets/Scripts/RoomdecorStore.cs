using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomdecorStore : MonoBehaviour, IRoomdecorStore
{
    [field: SerializeField] public Canvas canvas { get; private set; }
    [field: SerializeField] public bool repositionItem { get; private set; }
    [field: SerializeField] public bool changeItem { get; private set; }
    [field: SerializeField] public List<StoreItemData> itemSpriteData { get; private set; }
    [field: SerializeField] public GameObject myItemsScreen {  get; private set; }
    [field: SerializeField] public GameObject changeMyRoomScreen {  get; private set; }
    [field: SerializeField] public GameObject myRoomScreen {  get; private set; }
    [SerializeField] GameObject backButton_MyItemScreen;
    [SerializeField] GameObject backButton_MyItemScreen_IfRepositionTrue;
    [SerializeField] GameObject myItemButton;
    [SerializeField] GameObject storeButton;
    [SerializeField] TextMeshProUGUI coinText;

    private void OnEnable()
    {
        RegisterService();
        int c = PlayerPrefs.GetInt("Coins");
        coinText.text = c.ToString();
    }

    private void OnDisable()
    {
        UnRegisterService();
    }

    public void ChangeItemSprite(Image item, Sprite sprite)
    {
        item.sprite = sprite;
    }

    public void RepositionItem(bool val)
    {
        repositionItem = val;
        GameEvents.RoomDecorEvents.SetRoomDecorPermissionStatus.Raise(!val);
    }
    public void CheckBackButtonEnableDisable()
    {
        if (repositionItem)
        {
            backButton_MyItemScreen_IfRepositionTrue.SetActive(true);
            backButton_MyItemScreen.SetActive(false);
        }
        else
        {
            backButton_MyItemScreen.SetActive(true);
            backButton_MyItemScreen_IfRepositionTrue.SetActive(false);
        }
    }
    public void ChangeItem(bool val)
    {
        changeItem = val;
    }
    public void EnableDisableChangeRoomUiParent(bool val)
    {
        changeMyRoomScreen.SetActive(val);
    }
    public void EnableDisableMItemsScreen(bool val)
    {
        storeButton.SetActive(!val);
        myItemButton.SetActive(!val);
        myItemsScreen.SetActive(val);
        CheckBackButtonEnableDisable();
    }
    public void EnableDisableMyRoomScreen(bool val)
    {
        myRoomScreen.SetActive(val);
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IRoomdecorStore>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IRoomdecorStore>(this);
    }
}