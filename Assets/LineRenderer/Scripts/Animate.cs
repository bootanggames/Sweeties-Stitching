using DG.Tweening;
using System;
using UnityEngine;

public class Animate : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float targetScale;
    [SerializeField] float orignalScale;
    [SerializeField] Ease ease;
    void Start()
    {
        StartTextAnimation();
    }
    public void StartTextAnimation()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.RaiseEvent(this.transform, orignalScale, targetScale, speed, ease);
    }

   
}
