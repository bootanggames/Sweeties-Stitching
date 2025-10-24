using DG.Tweening;
using UnityEngine;

public static partial class GameEvents
{
    public static class DoTweenAnimationHandlerEvents
    {
        public static readonly GameEvent<Transform, Vector3, float, Ease> onScaleTransform = new();
        public static readonly GameEvent<Transform, float, float, float, Ease> onScaleAnimation = new();
        public static readonly GameFunc<Transform, Transform, float, Ease,Tween> onMoveToTargetAnimation = new();
    }
}

