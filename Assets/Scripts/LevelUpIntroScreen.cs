using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUpIntroScreen : MonoBehaviour
{
    [SerializeField] GameObject[] plishieObj;
    [SerializeField] float speed;
    [SerializeField] TextMeshProUGUI levelNumberText;
    [SerializeField] GameObject renderTexture;
    ILevelUpScreen levelUpScreen;
    [SerializeField]int index = 0;
    GameObject currentPlushie;
    Tween currentTween;
    [field: SerializeField] public List<PlushieSpriteContainer> pageSliderContainer { get; private set; }

    private void OnEnable()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        levelNumberText.text = "Level "+(levelIndex + 1).ToString();
        levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        renderTexture.SetActive(true);
        levelUpScreen.homeScreen.SetVolumeForBgMusic(0.5f);
        foreach (PlushieSpriteContainer container in pageSliderContainer)
        {
            if (container.levelId.Equals(levelIndex + 1))
            {
                for (int i = 0; i < plishieObj.Length; i++)
                {
                    LevelUpPlushieInfo plushieInfo = plishieObj[i].GetComponent<LevelUpPlushieInfo>();
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
            this.gameObject.SetActive(false);
            foreach (GameObject g in plishieObj)
            {
                g.SetActive(false);
                g.transform.localScale = Vector3.zero;
            }
            index = 0;
        }
    }
    void EnableInSequence()
    {
        if (index >= plishieObj.Length)
        {
            Invoke(nameof(ResetScreen), 1.5f);
            return;
        }
        currentPlushie = plishieObj[index];
        currentPlushie.SetActive(true);
        currentTween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(currentPlushie.transform, new Vector3(1.5f,1.5f,1.5f), speed, Ease.Linear);
        
        if(currentTween != null )
        {
            currentTween.OnComplete(() =>
            {
                currentTween.Kill();
                currentTween = null;
                plishieObj[index].transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    LevelUpPlushieInfo plushie = currentPlushie.GetComponent<LevelUpPlushieInfo>();
                    plushie.effect.SetActive(true);
                    plushie.effect.GetComponent<ParticleSystem>().Play();
                    plushie.PlaySound();
                    index++;
                    EnableInSequence();
                });
               
            });
           
        }
       
    }
}
