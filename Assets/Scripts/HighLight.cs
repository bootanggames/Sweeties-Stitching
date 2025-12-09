using DG.Tweening;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    Tween fadeInOut;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Ease ease;
    [SerializeField] float speed;
    private void OnEnable()
    {
        StartFadeInOutEffect(1);
    }
    void StartFadeInOutEffect(float endVal)
    {
        fadeInOut = GameEvents.DoTweenAnimationHandlerEvents.onUIHighLight.Raise(canvasGroup, endVal, speed, ease);
        if(fadeInOut != null )
        {
            fadeInOut.OnUpdate(() =>
            {
                if (canvasGroup.alpha >= 0.99f)
                    canvasGroup.alpha = 1;
            });
            fadeInOut.OnComplete(() =>
            {
                fadeInOut.Kill();
                fadeInOut = null;
                if (endVal == 1)
                    StartFadeInOutEffect(0);
                else
                    StartFadeInOutEffect(1);
            });
        }
    }
}
