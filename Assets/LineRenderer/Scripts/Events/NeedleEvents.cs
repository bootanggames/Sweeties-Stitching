using UnityEngine;

public static partial class GameEvents
{
    public static class NeedleEvents
    {
        public static readonly GameEvent<Vector3> OnNeedleMovement = new();
        public static readonly GameFunc<Vector3> OnFetchingNeedlePosition = new();
        public static readonly GameFunc<Transform> onGettingNeedleTransform = new();
    }
}

