using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecorItemsInventory : ScreenWithSelectableButtons<DecoreItemStoreButton>
{
    [SerializeField] private GameObject _container;
    [SerializeField] private DecorItemRepositorySO _decorItemRepository;
    [SerializeField] AudioSource _audiosSource;
    DecorItemName itemName;
    [SerializeField] GameObject useButtonObj;
    private DecorItemType _itemType = DecorItemType.BED;

    private void Start()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.Register(OnShowDecorItemsInventory);

    }

    void OnDestroy()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.UnRegister(OnShowDecorItemsInventory);
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
    public void StopSound()
    {
        if (AudiosSourceContainer.instance)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.roomInventoryScreen);
            AudioClip clip = SoundManager.instance.audioClips.bgMusic;
            SoundManager.instance.PlaySound(AudiosSourceContainer.instance.homeScreen, clip, true, false, 1, true);
        }
    }
    public void ShowWithBeds()
    {
        OnShowDecorItemsInventory(DecorItemType.BED);
    }
    public void ShowWithFloor()
    {
        OnShowDecorItemsInventory(DecorItemType.FLOOR);
    }
    private void OnShowDecorItemsInventory(DecorItemType decorItemType)
    {
        _itemType = decorItemType;
        Debug.LogError($"OnShowDecorItemsInventory {decorItemType}");
        SpawnButtons();
        _container.SetActive(true);
    }
    
    protected override void SpawnButtons()
    {
        base.SpawnButtons();
        SpawnButtons(_itemType);
    }

    public void ShowByType(DecorItemType decorItemType)
    {
        OnShowDecorItemsInventory(decorItemType);
    }
    
    private void SpawnButtons(DecorItemType decorItemType)
    {
        List<DecorIteamMetaDataSO> items = _decorItemRepository.GetItemsByType(decorItemType);
        foreach (DecorIteamMetaDataSO item in items)
        {
            UIContext context = new()
            {
                ImageToSet = item.ItemIcon,
                LabelToSet = item.DisplayName,
                ID = (int)item.ItemName,
            };

            SpawnButton(context);
        }
    }

    protected override void OnItemButtonClicked(UIContext context)
    {
        itemName = (DecorItemName)context.ID;
        useButtonObj.SetActive(true);
        //GameEvents.RoomDecorEvents.DecorItemSelected.Raise(itemName, _itemType);
        //_container.SetActive(false);
    }

    public void UseButton()
    {
        GameEvents.RoomDecorEvents.DecorItemSelected.Raise(itemName, _itemType);
        useButtonObj.SetActive(false);
        _container.SetActive(false);
    }
}
