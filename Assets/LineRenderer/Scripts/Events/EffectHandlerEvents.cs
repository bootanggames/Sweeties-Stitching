using UnityEngine;

public static partial class GameEvents
{
    public static class EffectHandlerEvents
    {
        public static readonly GameEvent<Transform> onSelectionEffect = new ();
    }
}

