using DG.Tweening;
using System.Collections;
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
    [SerializeField] bool levelIntro;
    ILevelUpScreen levelUpScreen;
    private void OnEnable()
    {
        levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
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
                    StartCoroutine(LevelUpScreenActivation());
                else if (levelIntro)
                    LevelIntroScreen();
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
        tween.Kill();
        tween = null;
        PlaySound();
        //FireworksParticles();
   
        Invoke(nameof(FireworksParticles), 0.25f);
    }

    void FireworksParticles()
    {
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


    IEnumerator LevelUpScreenActivation()
    {
        yield return new WaitForSeconds(1);
        this.transform.GetComponent<Image>().enabled = false;
        this.transform.localScale = Vector3.zero;
        if (levelUpScreen != null)
        {
            levelUpScreen.PlayCelebrationSound();
            levelUpScreen.levelUpScreen.SetActive(false);
            levelUpScreen.levelUpFadeScreen.SetActive(true);
            Invoke(nameof(NextLevelPanel), 0.25f);
        }
        StopCoroutine(LevelUpScreenActivation());
        //CancelInvoke(nameof(LevelUpScreenActivation));
    }
    void NextLevelPanel()
    {
        if (levelUpScreen != null)
        {
            int levelIndex = PlayerPrefs.GetInt("Level");

            levelUpScreen.NextPage(levelIndex);

        }

        Invoke(nameof(EnableWordUnlockedPlushies), 0.25f);
        //Invoke(nameof(LevelIntroScreen), 2.5f);
        CancelInvoke(nameof(NextLevelPanel));
    }
    void EnableWordUnlockedPlushies()
    {
        levelUpScreen.unlockedPlushieWord.SetActive(true);
        CancelInvoke(nameof(EnableWordUnlockedPlushies));
    }
    void LevelIntroScreen()
    {
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
