using DG.Tweening;
using System.Runtime.Versioning;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScaleOutObject : MonoBehaviour
{
    Tween tween;
    [SerializeField] Vector3 targetScale;
    [SerializeField] float speed;
    [SerializeField] Ease ease;
    [SerializeField] bool startGame;
    [SerializeField] bool levelUp;
    private void OnEnable()
    {
        ScaleOut();
    }
    private void OnDisable()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Kill();
            tween = null;
        }
    }
    void ScaleOut()
    {
        tween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(this.transform, targetScale, speed, ease);
        if (tween != null)
        {
            tween.OnComplete(() =>
            {
                if (startGame)
                    Invoke("StartGame", 0.25f);
                else if (levelUp)
                    OnLevelUp();
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
        this.gameObject.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(false);
        CancelInvoke("StartGame");
    }

    void GameComplete()
    {
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
        //FireworksParticles();
   
        Invoke(nameof(FireworksParticles), 0.25f);
    }

    void FireworksParticles()
    {
        //var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        //if (canvasHandler != null)
        //    canvasHandler.confettiEffectCanvas.SetActive(true);
        tween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(this.transform, Vector3.zero, speed, ease);
        if (tween != null)
        {
            tween.OnComplete(() =>
            {
                //GameEvents.EffectHandlerEvents.onSewnCompletely.RaiseEvent();
            });
        }
        CancelInvoke(nameof(FireworksParticles));
    }
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }

    void OnLevelUp()
    {
        var levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        if (levelUpScreen != null)
        {
            levelUpScreen.PlayLevelUpSound();
            levelUpScreen.confettiCameraRenderObj.SetActive(true);
        }
        Invoke(nameof(ConfettiEffect),0.5f);
    }
    void ConfettiEffect()
    {
        var levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        if (levelUpScreen != null)
            levelUpScreen.levelUpCamera.SetActive(false);
        GameEvents.EffectHandlerEvents.onSewnCompletely.Raise();
        Invoke(nameof(LevelUpScreenActivation), 3.0f);
    }
    void LevelUpScreenActivation()
    {
        this.transform.GetComponent<Image>().enabled = false;
        var levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        if (levelUpScreen != null)
        {
            levelUpScreen.PlayCelebrationSound();
            levelUpScreen.levelUpFadeScreen.SetActive(true);
            Invoke(nameof(NextLevelPanel), 1.0f);
        }
        CancelInvoke(nameof(LevelUpScreenActivation));
    }
    void NextLevelPanel()
    {
        var levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        if (levelUpScreen != null)
        {
            int levelIndex = PlayerPrefs.GetInt("Level");

            if (levelIndex >= levelUpScreen.pageSliderContainer.Count || levelIndex == 0)
                levelUpScreen.PrevPage();
            else
                levelUpScreen.NextPage();

        }

        Invoke(nameof(LevelIntroScreen), 2.5f);
        CancelInvoke(nameof(NextLevelPanel));
    }

    void LevelIntroScreen()
    {
        var levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        if (levelUpScreen != null)
        {
            levelUpScreen.levelUpFadeScreen.SetActive(false);
            levelUpScreen.levelUpIntroScreen.SetActive(true);
            levelUpScreen.PlayLevelUpSongSound();
            this.GetComponent<Image>().enabled = true;
            this.gameObject.SetActive(false);
        }
        CancelInvoke(nameof(LevelIntroScreen));
    }
}
