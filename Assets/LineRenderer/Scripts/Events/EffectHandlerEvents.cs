using UnityEngine;

public static partial class GameEvents
{
    public static class EffectHandlerEvents
    {
        public static readonly GameEvent<Transform> onSelectionEffect = new ();
        public static readonly GameQuery<Transform,GameObject> onGetInstantiatedEffect = new ();
        public static readonly GameQuery<Transform,GameObject> onPartCompleteEffect = new ();
        public static readonly GameEvent onSewnCompletely = new();
        public static readonly GameEvent<Transform> onSparkleTrailEffect = new();
        public static readonly GameQuery<Transform, GameObject> onSparkleTrailEffectOnCompletion = new();

    }
}

