using UnityEngine;

public partial class GameEvents
{
    public class ThreadEvents
    {
        public readonly static GameEvent<Vector3> onInitialiseRope = new();
        public readonly static GameEvent<Vector3> onAddingPositionToRope = new();

    }
}

