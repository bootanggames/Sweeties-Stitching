using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecorItemsInventory : ScreenWithSelectableButtons<DecoreItemStoreButton>
{
    [SerializeField] private GameObject _container;
    [SerializeField] private DecorItemRepositorySO _decorItemRepository;

    private DecorItemType _itemType = DecorItemType.BED;

    private void Start()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.Register(OnShowDecorItemsInventory);
    }

    void OnDestroy()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.UnRegister(OnShowDecorItemsInventory);
    }

    public void ShowWithBeds()
    {
        OnShowDecorItemsInventory(DecorItemType.BED);
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
        DecorItemName itemName = (DecorItemName)context.ID;
        GameEvents.RoomDecorEvents.DecorItemSelected.Raise(itemName, _itemType);
        _container.SetActive(false);
    }
}
