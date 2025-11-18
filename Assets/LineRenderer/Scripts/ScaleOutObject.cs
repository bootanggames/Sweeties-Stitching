using DG.Tweening;
using UnityEngine;

public class ScaleOutObject : MonoBehaviour
{
    Tween tween;
    [SerializeField] Vector3 targetScale;
    [SerializeField] float speed;
    [SerializeField] Ease ease;
    [SerializeField] bool startGame;
    private void OnEnable()
    {
        ScaleOut();
    }
    void ScaleOut()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Kill();
            tween = null;
        }
        tween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(this.transform, targetScale, speed, ease);
        if (tween != null)
        {
            tween.OnComplete(() =>
            {
                if (startGame)
                    Invoke("StartGame", 0.5f);
                else
                    GameComplete();
            });
        }
    }

    void StartGame()
    {
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
            canvasHandler.TapToStart();

        tween.Kill();
        this.gameObject.SetActive(false);
        CancelInvoke("StartGame");
    }

    void GameComplete()
    {
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
            canvasHandler.confettiEffectCanvas.SetActive(true);
        LevelsHandler.instance.currentLevelMeta.DeactivateAllThreads();
        LevelsHandler.instance.currentLevelMeta.gameObject.SetActive(false);
        LevelsHandler.instance.currentLevelMeta.sewnPlushie.SetActive(true);
        tween.Kill();
        tween = null;

        tween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(this.transform, Vector3.zero, speed, ease);
        if (tween != null)
        {
            tween.OnComplete(() =>
            {
                GameEvents.EffectHandlerEvents.onSewnCompletely.RaiseEvent();
            });
        }
    }
   
}
