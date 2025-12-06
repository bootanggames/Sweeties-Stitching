using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecorItemsStore : ScreenWithSelectableButtons<DecoreItemStoreButton>
{
    private Dictionary<DecorItemType, DecorItemName> _itemNames = new();
    
    protected override void OnItemButtonClicked(UIContext context)
    {
        DecorItemName itemName = (DecorItemName)context.ID;
        DecorItemType itemType = _itemNames.FirstOrDefault(item => item.Value == itemName).Key;

        GameEvents.RoomDecorEvents.DecorItemSelected.Raise(itemName, itemType);
    }
}
