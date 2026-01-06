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
    [field: SerializeField] public GameObject myItemsScreen {  get; private set; }
    [field: SerializeField] public GameObject changeMyRoomScreen {  get; private set; }
    [field: SerializeField] public GameObject myRoomScreen {  get; private set; }
    [field: SerializeField] public GameObject bedroom { get; private set; }

    [SerializeField] GameObject backButton_MyItemScreen;
    [SerializeField] GameObject myItemButton;
    [SerializeField] GameObject storeButton;
    [SerializeField] TextMeshProUGUI coinText;

    [field:SerializeField] public List<GameObject> roomScreenButtons {  get; private set; }
    UpdateRoom room;
    private void OnEnable()
    {
        RegisterService();
        int c = PlayerPrefs.GetInt("Coins");
        coinText.text = c.ToString();
        room = bedroom.GetComponent<UpdateRoom>();
       
    }
    public void OpenScreen()
    {
        int openDecor = PlayerPrefs.GetInt("OpenRoomDecor");
        if (openDecor == 1)
        {
            MyRoomButton();
            PlayerPrefs.SetInt("OpenRoomDecor", 0);
        }
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

    
    public void MyItemsButton(bool val)
    {

        if (repositionItem)
        {
            room.UpdateRoomState("move");
            EnableDisableMItemsScreen(val);
            EnableDisableChangeRoomUiParent(!val);
        }
        else
        {
            room.UpdateRoomState("decor");
            EnableDisableMyRoomScreen(!val);
            EnableDisableMItemsScreen(val);
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
        //storeButton.SetActive(val);
        //myItemButton.SetActive(val);
        myItemsScreen.SetActive(val);
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

    public void ChangeBedroomComponents(bool val)
    {
        if (room != null)
        {
            foreach(RoomItem g in room.roomitem)
            {
                g.EnableDisableItemComponents(val);
            }
        }
    }
    public void StopSound()
    {
        if (AudiosSourceContainer.instance)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.roomInventoryScreen);
            AudioClip clip = SoundManager.instance.audioClips.bgMusic;
            SoundManager.instance.PlaySound(AudiosSourceContainer.instance.homeScreen, clip, true, false, 1, true);
        }
    }
    public void PlaySound()
    {
        if (AudiosSourceContainer.instance)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.homeScreen);
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.plushieInventoryScreen);
            AudioClip clip = SoundManager.instance.audioClips.roomInventoryScreenSound;
            SoundManager.instance.PlaySound(AudiosSourceContainer.instance.roomInventoryScreen, clip, true, false, 1, true);
        }
    }
    public void CloseMyRoom()
    {
        StopSound();
        MainMenuHandler.instance.UpdateScreen("home");
        room.UpdateRoomState("home");
        ChangeBedroomComponents(false);
        RepositionItem(false);
        ChangeItem(false);
    }
    public void MyRoomButton()
    {
        PlaySound();
        EnableDisableMyRoomScreen(true);
        EnableDisableMItemsScreen(false);
        EnableDisableChangeRoomUiParent(false);
        ChangeBedroomComponents(true);
        room.UpdateRoomState("decor");
        MainMenuHandler.instance.UpdateScreen("roomdecor");
        foreach(GameObject g in roomScreenButtons)
        {
            g.SetActive(true);
        }
    }
}