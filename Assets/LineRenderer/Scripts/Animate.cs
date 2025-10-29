using DG.Tweening;
using System;
using UnityEngine;

public class Animate : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float targetScale;
    [SerializeField] float orignalScale;
    [SerializeField] Ease ease;
    [SerializeField] bool startGame;
    Tween t;
    void Start()
    {
        StartTextAnimation();
    }
    public void StartTextAnimation()
    {
        if (startGame)
            t = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(this.transform, new Vector3(targetScale, targetScale, targetScale), speed, Ease.Linear);
        else
            GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.RaiseEvent(this.transform, orignalScale, targetScale, speed, ease);

        if (t != null)
        {
            t.OnComplete(() =>
            {
                Invoke("StartGame", 0.5f);

            });
        }
    }

    void StartGame()
    {
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
            canvasHandler.TapToStart();

        t.Kill();
        this.gameObject.SetActive(false);
        CancelInvoke("StartGame");
    }
}
