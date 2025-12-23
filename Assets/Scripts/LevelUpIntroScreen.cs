using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUpIntroScreen : MonoBehaviour
{
    [SerializeField] GameObject[] plishieObj;
    [SerializeField] float speed;
    [SerializeField] TextMeshProUGUI levelNumberText;
    //[SerializeField] GameObject renderTexture;
    ILevelUpScreen levelUpScreen;
    [SerializeField]int index = 0;
    GameObject currentPlushie;
    Tween currentTween;
    [SerializeField] AudioSource source;
    [field: SerializeField] public List<PlushieSpriteContainer> pageSliderContainer { get; private set; }
    IJackpotHandler jackpotHandler;
    private void OnEnable()
    {

        int levelIndex = PlayerPrefs.GetInt("Level");
        levelNumberText.text = "Level "+(levelIndex + 1).ToString();
        levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
        //renderTexture.SetActive(true);
        //levelUpScreen.levelUpCamera.SetActive(false);
        levelUpScreen.homeScreen.SetVolumeForBgMusic(0.5f);
        foreach (PlushieSpriteContainer container in pageSliderContainer)
        {
            if (container.levelId.Equals(levelIndex + 1))
            {
                for (int i = 0; i < plishieObj.Length; i++)
                {
                    LevelUpPlushieInfo plushieInfo = plishieObj[i].GetComponentInChildren<LevelUpPlushieInfo>();
                    plushieInfo.plushie.sprite = container.plushieDetail[i].plushie;
                    plushieInfo.plushieName.text = container.plushieDetail[i].plushieName;
                }
                break;
            }
        }
        EnableInSequence();
    }
    void ResetScreen()
    {
        if (levelUpScreen != null)
        {
            levelUpScreen.StopSound();
            levelUpScreen.homeScreen.SetVolumeForBgMusic(1);
            levelUpScreen.homeScreen.EnableSound();
            levelUpScreen.renderTextureImageObj.SetActive(false);
            levelUpScreen.levelUpScreen.SetActive(false);
            levelUpScreen.confettiCameraRenderObj.SetActive(false);
            levelUpScreen.homeCanvas.SetActive(true);
            levelUpScreen.levelUpIntroScreen.SetActive(false);
            //this.gameObject.SetActive(false);
            foreach (GameObject g in plishieObj)
            {
                LevelUpPlushieInfo plushie = g.GetComponentInChildren<LevelUpPlushieInfo>();
                //plushie.effect.SetActive(false);
                plushie.transform.localScale = Vector3.zero;
                g.SetActive(false);
            }
            jackpotHandler.ShowJackPotScreen();
            index = 0;
        }
    }

    void CoinBagExplodeSound()
    {
        SoundManager.instance.PlaySound(source, SoundManager.instance.audioClips.coinBagExploding, false, false, 1, false);
    }
    bool once = false;
    void EnableInSequence()
    {
        //if (!once)
        //{
        //    CoinBagExplodeSound();
        //    once = true;
        //}
        if (index >= plishieObj.Length)
        {
            Invoke(nameof(ResetScreen), 1.5f);
            return;
        }
    
        currentPlushie = plishieObj[index];
        currentPlushie.SetActive(true);
        if (index > 0)
            plishieObj[index - 1].GetComponentInChildren<LevelUpPlushieInfo>().effect.SetActive(false);
        LevelUpPlushieInfo plushie = currentPlushie.GetComponentInChildren<LevelUpPlushieInfo>();

        currentTween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(plushie.transform, new Vector3(1.5f,1.5f,1.5f), speed, Ease.Linear);

        if (currentTween != null )
        {
            currentTween.OnComplete(() =>
            {
                once = false;

                currentTween.Kill();
                currentTween = null;
                plushie.effect.SetActive(true);
                plushie.effect.GetComponent<ParticleSystem>().Play();
                plushie.PlaySound();
                CoinBagExplodeSound();
                //Debug.LogError("sound");

                plushie.transform.DOScale(Vector3.one, speed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    //plushie.effect.SetActive(false);
                    index++;
                    EnableInSequence();
                });
               
            });
        }
    }
   
}
