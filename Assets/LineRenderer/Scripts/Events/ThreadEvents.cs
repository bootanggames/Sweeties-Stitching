using UnityEngine;

public static partial class GameEvents
{
    public static class ThreadEvents
    {
        public static readonly GameEvent<bool,Vector2> onInstantiatingThread = new();
        public static readonly GameEvent<Vector2> onInitialiseRope = new();
        public static readonly GameEvent<Vector2> onAddingPositionToRope = new();
        public static readonly GameEvent<SewPoint> onCreatingConnection = new();
        public static readonly GameEvent onUpdateLinkMovement = new();
        public static readonly GameEvent onEmptyList_DetectingPoints = new();
        public static readonly GameEvent<bool> setThreadInput = new();
        public static readonly GameEvent<bool> onSetFreeformMovementValue = new();
        public static readonly GameEvent onResetThreadInput = new();

    }
}

