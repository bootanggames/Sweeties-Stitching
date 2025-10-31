using DG.Tweening;
using UnityEngine;

public class DoTweenAnimationHandler : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.RegisterEvent(ScaleTransform);
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.RegisterEvent(ScaleInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.RegisterEvent(MoveToTarget);
        GameEvents.DoTweenAnimationHandlerEvents.onUIHighLight.RegisterEvent(FadeInOut);
    }
    private void OnDisable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.UnregisterEvent(ScaleTransform);
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.UnregisterEvent(ScaleInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.UnregisterEvent(MoveToTarget);
        GameEvents.DoTweenAnimationHandlerEvents.onUIHighLight.UnregisterEvent(FadeInOut);
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

    Tween ScaleTransform(Transform t,Vector3 targetScale, float speed, Ease ease)
    {
        Tween scaleTween = null;
        scaleTween = t.DOScale(targetScale, speed).SetEase(ease);
        return scaleTween;
    }

    Tween FadeInOut(CanvasGroup cg, float endVal, float speed, Ease ease)
    {
        return DOTween.To(() => cg.alpha, x => cg.alpha = x, endVal, speed).SetEase(ease);
    }
}
