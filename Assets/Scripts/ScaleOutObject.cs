using DG.Tweening;
using System.Collections;
using System.Runtime.Versioning;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class ScaleOutObject : MonoBehaviour
{
    Tween tween;
    [SerializeField] Vector3 targetScale;
    [SerializeField] float speed;
    [SerializeField] Ease ease;
    [SerializeField] bool startGame;
    [SerializeField] bool levelUp;
    [SerializeField] bool levelIntro;
    [SerializeField] bool jackPotWord;
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
                else if (jackPotWord)
                    Invoke(nameof(JackPot), 1);
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
        Invoke(nameof(ScaleIn), 0.25f);
    }

    void ScaleIn()
    {
        tween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(this.transform, Vector3.zero, speed, ease);
        if (tween != null)
        {
            tween.OnComplete(() =>
            {
                //GameEvents.EffectHandlerEvents.onSewnCompletely.RaiseEvent();
                
            });
        }
        CancelInvoke(nameof(ScaleIn));
    }
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.plushieSewnVoice;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
        this.AddComponent<AudioSource>();
        AudioSource sewnWordAudio = this.GetComponent<AudioSource>();
        SoundManager.instance.PlaySound(sewnWordAudio, _clip, false, false, 1, false);
    }


    IEnumerator LevelUpScreenActivation()
    {
        yield return new WaitForSeconds(0.5f);
        this.transform.GetComponent<Image>().enabled = false;
        this.transform.localScale = Vector3.zero;
        if (levelUpScreen != null)
        {
            levelUpScreen.PlayCelebrationSound();
            levelUpScreen.levelUpData.levelUpScreen.SetActive(false);
            levelUpScreen.levelUpData.levelUpFadeScreen.SetActive(true);
            levelUpScreen.DisableParticleEffects();
            NextLevelPanel();
            //Invoke(nameof(NextLevelPanel), 0.2f);
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
        EnableWordUnlockedPlushies();
        //Invoke(nameof(EnableWordUnlockedPlushies), 0.2f);
        //Invoke(nameof(LevelIntroScreen), 2.5f);
        CancelInvoke(nameof(NextLevelPanel));
    }
    void EnableWordUnlockedPlushies()
    {
        levelUpScreen.levelUpData.unlockedPlushieWord.SetActive(true);
        CancelInvoke(nameof(EnableWordUnlockedPlushies));
    }
    void LevelIntroScreen()
    {
        if (levelUpScreen != null)
        {
            levelUpScreen.levelUpData.levelUpFadeScreen.SetActive(false);
            levelUpScreen.levelUpData.levelUpIntroScreen.SetActive(true);
            //levelUpScreen.homeCanvas.SetActive(false);
            levelUpScreen.PlayLevelUpSongSound();
            this.GetComponent<Image>().enabled = true;
            this.gameObject.SetActive(false);
        }
        CancelInvoke(nameof(LevelIntroScreen));
    }

    void JackPot()
    {
        JackpotMachine j_machine = gameObject.GetComponentInParent<JackpotMachine>();
        j_machine.UIObject.SetActive(true);
        j_machine.jackPotMachineObject.SetActive(true);
        j_machine.jackpotWord.SetActive(false);
        //GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
    }

    //public void JackPotScreenSound()
    //{
    //    AudioClip clip = SoundManager.instance.audioClips.youHaveJackpot;
    //    SoundManager.instance.PlaySound(SoundManager.instance.audioSource, clip, false, false, 1, false);
    //}
}
