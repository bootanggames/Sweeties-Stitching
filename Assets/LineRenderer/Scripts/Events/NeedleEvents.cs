using UnityEngine;

public partial class GameEvents
{
    public class NeedleEvents
    {
        public readonly static GameEvent<Vector3> OnNeedleMovement = new();
        public readonly static GameFunc<Vector3> OnFetchingLastNeedlePosition = new();
    }
}

