using DG.Tweening;
using UnityEngine;

public class DoTweenAnimationHandler : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.RegisterEvent(ScaleTransform);
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.RegisterEvent(ScaleInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.RegisterEvent(MoveToTarget);
    }
    private void OnDisable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.UnregisterEvent(ScaleTransform);
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.UnregisterEvent(ScaleInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.UnregisterEvent(MoveToTarget);
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

    Tween MoveToTarget(Transform obj, Transform target, float moveSpeed, Ease ease)
    {
        Tween moveTween = null;
        moveTween = obj.DOMove(target.position, moveSpeed).SetEase(ease);
        return moveTween;
    }

    void ScaleTransform(Transform t,Vector3 targetScale, float speed, Ease ease)
    {
        t.DOScale(targetScale, speed).SetEase(ease).OnComplete(() =>
        {
            //t.DOPause();
        });
    }
}
