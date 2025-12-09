using UnityEngine;

public static partial class GameEvents
{
    public static class RoomDecorEvents
    {
        public static readonly GameEvent<DecorItemName,DecorItemType> DecorItemSelected = new();
        public static readonly GameEvent<bool> SetRoomDecorPermissionStatus = new();
    }
}

