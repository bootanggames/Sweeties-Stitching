using UnityEngine;

public class DecoreItemStoreButton : GameStoreButton
{
    public DecorItemName DecorItemName { get; private set; }
    public DecorItemType DecorItemType { get; private set; }
    
    public void SetDecorItemData(DecorItemName decorItemName,DecorItemType decorItemType)
    {
        DecorItemName = decorItemName;
        DecorItemType = decorItemType;
    }
}
