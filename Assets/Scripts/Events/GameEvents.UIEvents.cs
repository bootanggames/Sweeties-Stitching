using UnityEngine;

public static partial class GameEvents
{
    public static class UIEvents
    {
        public static readonly GameEvent<ItemType> ShowDecorItemsInventory = new();
    }
}

