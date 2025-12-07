using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecorItemsInventory : ScreenWithSelectableButtons<DecoreItemStoreButton>
{
    [SerializeField] private GameObject _container;
    [SerializeField] private DecorItemRepositorySO _decorItemRepository;
    private Dictionary<DecorItemType, DecorItemName> _itemNames = new();

    private DecorItemType _itemType = DecorItemType.BED;

    private void Start()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.Register(OnShowDecorItemsInventory);
    }

    void OnDestroy()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.UnRegister(OnShowDecorItemsInventory);
    }

    private void OnShowDecorItemsInventory(DecorItemType decorItemType)
    {
        _itemType = decorItemType;
        SpawnButtons();
        _container.SetActive(true);
    }
    
    protected override void SpawnButtons()
    {
        base.SpawnButtons();
        SpawnButtons(_itemType);
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
        DecorItemType itemType = _itemNames.FirstOrDefault(item => item.Value == itemName).Key;

        GameEvents.RoomDecorEvents.DecorItemSelected.Raise(itemName, itemType);
        _container.SetActive(false);
    }
}
