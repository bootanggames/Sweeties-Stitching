using UnityEngine;

public static partial class GameEvents
{
    public static class PointConnectionHandlerEvents
    {
        public static readonly GameEvent<SewPoint, LineRenderer> onFetchingPoints = new();
    }
}

