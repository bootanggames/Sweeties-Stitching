using UnityEngine;

public static partial class GameEvents
{
    public static class NeedleDetectorEvents
    {
        public static readonly GameEvent<float> onSetRadiusValue = new ();
    }
}

