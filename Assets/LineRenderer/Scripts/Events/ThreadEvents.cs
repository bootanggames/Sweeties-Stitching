using UnityEngine;

public static partial class GameEvents
{
    public static class ThreadEvents
    {
        public static readonly GameEvent<Vector2> onInitialiseRope = new();
        public static readonly GameEvent<Vector2> onAddingPositionToRope = new();

    }
}

