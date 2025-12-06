using UnityEngine;

public class DecorItemsStore : ScreenWithSelectableButtons<DecoreItemStoreButton>
{
    protected override void OnItemButtonClicked(UIContext context)
    {
        DecorItemName item = (DecorItemName)context.ID;
        
    }
}
