using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static class PointConnectionHandlerEvents
    {
        public static readonly GameEvent<List<Transform>> onFetchingPoints = new();
        public static readonly GameEvent onStopTweens = new();
        public static readonly GameEvent<float> onUpdatingPullSpeed = new();
        public static readonly GameFunc<float> onGettingMaxSpeed = new();
    }
}

