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
        if (tween != null && tween.IsActive())
        {
            tween.Kill();
            tween = null;
        }
        ScaleOut();
    }
    void ScaleOut()
    {
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
        //LevelsHandler.instance.currentLevelMeta.DeactivateAllThreads();
        //LevelsHandler.instance.currentLevelMeta.sewnPlushie.SetActive(true);
        //LevelsHandler.instance.currentLevelMeta.gameObject.SetActive(false);
        //foreach (Connections c in LevelsHandler.instance.currentLevelMeta.cleanConnection)
        //{
        //    Destroy(c.line.gameObject);
        //}
        //LevelsHandler.instance.currentLevelMeta.cleanConnection.Clear();
        //foreach(GameObject g in LevelsHandler.instance.currentLevelMeta.crissCrossObjList)
        //{
        //    Destroy(g);
        //}
        //LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Clear();
        tween.Kill();
        tween = null;
        PlaySound();

        tween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(this.transform, Vector3.zero, speed, ease);
        if (tween != null)
        {
            tween.OnComplete(() =>
            {
                GameEvents.EffectHandlerEvents.onSewnCompletely.RaiseEvent();
            });
        }
    }
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
}
