using UnityEngine;

public class DecoreItemStoreButton : GameStoreButton
{
    public ItemName DecorItemName { get; private set; }
    public ItemType DecorItemType { get; private set; }
    
    public void SetDecorItemData(ItemName decorItemName, ItemType decorItemType)
    {
        DecorItemName = decorItemName;
        DecorItemType = decorItemType;
    }
}
