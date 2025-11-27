using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public static partial class GameEvents
{
    public static class DoTweenAnimationHandlerEvents
    {
        public static readonly GameFunc<Transform, Vector3, float, Ease, Tween> onScaleTransform = new();
        public static readonly GameEvent<Transform, float, float, float, Ease> onScaleAnimation = new();
        public static readonly GameFunc<Transform, Vector3, float, Ease,Tween> onMoveToTargetAnimation = new();
        public static readonly GameFunc<RectTransform, RectTransform, float, Ease,Tween> onMoveToRectTargetAnimation = new();
        public static readonly GameFunc<CanvasGroup, float, float, Ease,Tween> onUIHighLight = new();
        public static readonly GameFunc<Transform, float, Ease, Tween> onSpining = new();
        public static readonly GameFunc<int,float, TextMeshProUGUI, Ease, Tween> onCountIncrement = new();

    }
}

