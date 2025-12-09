using UnityEngine;

public static partial class GameEvents
{
    public static class NeedleEvents
    {
        public static readonly GameEvent<Vector3> OnNeedleMovement = new();
        public static readonly GameQuery<Vector3> OnFetchingNeedlePosition = new();
        public static readonly GameQuery<Transform> onGettingNeedleTransform = new();
        public static readonly GameEvent<bool> onNeedleActiveStatusUpdate = new();
        public static readonly GameEvent<float, Vector3> onNeedleRotation = new();
    }
}

