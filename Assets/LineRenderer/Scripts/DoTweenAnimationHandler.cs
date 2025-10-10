using DG.Tweening;
using UnityEngine;

public class DoTweenAnimationHandler : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.RegisterEvent(ScaleInOut);
    }
    private void OnDisable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.UnregisterEvent(ScaleInOut);
    }
    void ScaleInOut(Transform t, float originalScale, float targetScale, float speed, Ease ease)
    {
        t.DOScale(targetScale, speed).SetEase(ease).OnComplete(() =>
        {
            t.DOScale(originalScale, speed).SetEase(ease).OnComplete(() =>
            {
                ScaleInOut(t, originalScale, targetScale, speed, ease);
            });
        });
    }
}
