using DG.Tweening;
using UnityEngine;

public class Animate : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float targetScale;
    [SerializeField] float orignalScale;
    [SerializeField] Ease ease;
    void Start()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.RaiseEvent(this.transform, orignalScale, targetScale, speed, ease);
    }

}
