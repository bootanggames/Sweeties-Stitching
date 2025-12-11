using DG.Tweening;
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
    private void OnEnable()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        levelNumberText.text = "Level "+(levelIndex + 1).ToString();
        levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        renderTexture.SetActive(true);
        levelUpScreen.homeScreen.SetVolumeForBgMusic(0.5f);
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
        currentTween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(currentPlushie.transform, Vector3.one, speed, Ease.Linear);
        
        if(currentTween != null )
        {
            currentTween.OnComplete(() =>
            {
                LevelUpPlushieInfo plushie = currentPlushie.GetComponent<LevelUpPlushieInfo>();
                plushie.effect.SetActive(true);
                plushie.effect.GetComponent<ParticleSystem>().Play();
                currentTween.Kill();
                currentTween = null;
                index++;
                EnableInSequence();
            });
           
        }
       
    }
}
