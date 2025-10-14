using UnityEngine;

public static partial class GameEvents
{
    public static class GameCompleteEvents
    {
        public static readonly GameEvent onGameComplete = new ();
        public static readonly GameEvent onGameWin = new ();
    }
}

