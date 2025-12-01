using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class DoTweenAnimationHandler : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.RegisterEvent(ScaleTransform);
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.RegisterEvent(ScaleInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.RegisterEvent(MoveToTarget);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToRectTargetAnimation.RegisterEvent(MoveToRectTarget);
        GameEvents.DoTweenAnimationHandlerEvents.onUIHighLight.RegisterEvent(FadeInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onSpining.RegisterEvent(Spining);
        GameEvents.DoTweenAnimationHandlerEvents.onCountIncrement.RegisterEvent(CoinsIncrement);

    }
    private void OnDisable()
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.UnregisterEvent(ScaleTransform);
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.UnregisterEvent(ScaleInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.UnregisterEvent(MoveToTarget);
        GameEvents.DoTweenAnimationHandlerEvents.onUIHighLight.UnregisterEvent(FadeInOut);
        GameEvents.DoTweenAnimationHandlerEvents.onSpining.UnregisterEvent(Spining);
        GameEvents.DoTweenAnimationHandlerEvents.onCountIncrement.UnregisterEvent(CoinsIncrement);
        GameEvents.DoTweenAnimationHandlerEvents.onMoveToRectTargetAnimation.UnregisterEvent(MoveToRectTarget);

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

    Tween MoveToTarget(Transform obj, Vector3 target, float moveSpeed, Ease ease)
    {
      
        Tween moveTween = null;
        moveTween = obj.DOMove(target, moveSpeed).SetEase(ease);
     

        return moveTween;
    }
    Tween MoveToRectTarget(RectTransform obj, RectTransform target, float moveSpeed, Ease ease)
    {
        Tween moveTween = null;
        moveTween = obj.DOAnchorPos(target.position, moveSpeed).SetEase(ease);
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
    Tween Spining (Transform obj, float speed, Ease ease)
    {
        return obj.DORotate(new Vector3(0, 0, 360), speed, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear).Pause();
    }
    int actualAmount;
    Tween CoinsIncrement(int rewardAmount, float speed,TextMeshProUGUI coinsText, Ease ease)
    {
        actualAmount = PlayerPrefs.GetInt("Coins");
        int target = actualAmount + rewardAmount;
        return DOTween.To(() => actualAmount, x => 
        { 
            target = x; 
            coinsText.text = x.ToString();
            //int TotalEarned = actualAmount + x;
            PlayerPrefs.SetInt("Coins", x);
            var coinHandler = ServiceLocator.GetService<ICoinsHandler>();
            if (coinHandler != null)
                coinHandler.UpdateCoins(x);
        }, target, speed).SetEase(Ease.InOutBack);
    }
 
}
