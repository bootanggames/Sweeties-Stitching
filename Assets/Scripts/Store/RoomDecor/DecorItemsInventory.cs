using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DecorItemsInventory : ScreenWithSelectableButtons<DecoreItemStoreButton>
{
    [SerializeField] private GameObject _container;
    [SerializeField] private ItemRepository _decorItemRepository;
    [SerializeField] AudioSource _audiosSource;
    ItemName itemName;
    [SerializeField] GameObject useButtonObj;
    private ItemType _itemType = ItemType.BED;

    IRoomdecorStore roomdecorStore;
    public UIContext clickedItemContext {  get; private set; }
    StoreItemsInventory storeItems;
    private void Start()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.Register(OnShowDecorItemsInventory);
        roomdecorStore = ServiceLocator.GetService<IRoomdecorStore>();
        storeItems = this.GetComponent<StoreItemsInventory>();
    }

    void OnDestroy()
    {
        GameEvents.UIEvents.ShowDecorItemsInventory.UnRegister(OnShowDecorItemsInventory);
    }

   
    public List<DecoreItemStoreButton> ButtonsList()
    {
        return _buttons;
    }
    public void ShowWithBeds()
    {
        OnShowDecorItemsInventory(ItemType.BED);
    }
    public void ShowWithFloor()
    {
        OnShowDecorItemsInventory(ItemType.FLOOR);
    }
    private void OnShowDecorItemsInventory(ItemType decorItemType)
    {
        _itemType = decorItemType;
        //Debug.LogError($"OnShowDecorItemsInventory {decorItemType}");
        SpawnButtons();
        _container.SetActive(true);
    }
    
    protected override void SpawnButtons()
    {
        base.SpawnButtons();
        SpawnButtons(_itemType);
    }

    public void ShowByType(ItemType decorItemType)
    {
        OnShowDecorItemsInventory(decorItemType);
    }
    
    private void SpawnButtons(ItemType decorItemType)
    {
        List<ItemsMetaData> items = _decorItemRepository.GetItemsByType(decorItemType);
        foreach (ItemsMetaData item in items)
        {
            UIContext context = new()
            {
                ImageToSet = item.ItemIcon,
                LabelToSet = item.DisplayName,
                ID = (int)item.ItemName,
                Cost = (int)item.Price.CurrencyEntities[0].Value//----
            };
            SpawnButton(context);
        }
        if (storeItems)
            storeItems.shelfText.text = decorItemType.ToString();
    }
    public void DisableButtonOutline(UIContext context)
    {
        foreach (DecoreItemStoreButton b in  _buttons)
        {
            UIWidget uIWidget = b.GetComponent<UIWidget>();
            if (!context.ID.Equals(uIWidget.GetContextID()))
                   uIWidget.EnableDisableOutline(false);
            else
                uIWidget.EnableDisableOutline(true);
        }
    }
    protected override void OnItemButtonClicked(UIContext context)
    {
        itemName = (ItemName)context.ID;
        useButtonObj.SetActive(true);
        DisableButtonOutline(context);
        clickedItemContext = context;
        if(storeItems)
            storeItems.ButtonsActivity(context);
    }

    public void UseButton()
    {
        GameEvents.RoomDecorEvents.DecorItemSelected.Raise(itemName, _itemType);
        useButtonObj.SetActive(false);
        _container.SetActive(false);
        if (roomdecorStore != null)
        {
            roomdecorStore.MyItemsButton(false);
            foreach(GameObject g in roomdecorStore.roomScreenButtons)
            {
                g.SetActive(true);
            }
        }
    }

    
}
