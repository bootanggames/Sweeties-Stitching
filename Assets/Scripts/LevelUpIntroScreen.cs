using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpIntroScreen : MonoBehaviour
{
    [SerializeField] GameObject explosionEffectParent;
    [SerializeField] ParticleSystem[] explosionEffect;
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
    [SerializeField] Vector3 plushieTargetScaleIn;
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
            this.GetComponent<Image>().enabled = true;
            explosionEffectParent.SetActive(false);

            jackpotHandler.ShowJackPotScreen();
            index = 0;
        }
    }

    void CoinBagExplodeSound()
    {
        SoundManager.instance.PlaySound(source, SoundManager.instance.audioClips.coinBagExploding, false, false, 1, false);
    }
    void ExplodeSound()
    {
        SoundManager.instance.PlaySound(source, SoundManager.instance.audioClips.blast, false, false, 1, false);
    }

    IEnumerator PlushieSequence()
    {
        Sequence seq = DOTween.Sequence();
        int total = plishieObj.Length;
        int currentLevel = PlayerPrefs.GetInt("Level");
        LevelObjectivePageDetail objectivePage = MainMenuHandler.instance.levels.levelPage[currentLevel];
        List<GameObject> plushieIcons = new List<GameObject>();
        int index = 0;
        for (int i = total - 1; i >= 0; i--)
        {

            GameObject currentLevelObj = objectivePage.levelDetail[i].levelObject;
            LevelDetail ld = currentLevelObj.GetComponent<LevelDetail>();
            Transform parent = MainMenuHandler.instance.levels.GetComponentInParent<HomeScreenSound>().transform;
            ld.plushieImage.transform.SetParent(parent);
            Vector3 target = ld.plushieImage.GetComponent<RectTransform>().anchoredPosition3D;
            GameObject plushieIcon = plishieObj[i].GetComponentInChildren<LevelUpPlushieInfo>().gameObject;
            if (!plushieIcons.Contains(plushieIcon))
                plushieIcons.Add(plushieIcon);
            plushieIcon.transform.SetParent(parent);
            seq.Join(plushieIcon.transform.DOLocalMove(target, speed).SetEase(Ease.Linear));
            //seq.Join(GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.Raise(plushieIcon.transform, target, speed, Ease.Linear));
            seq.Join(GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(plushieIcon.transform, new Vector3(0.5f, 0.5f, 0.5f), speed, Ease.Linear));
        }
        seq.OnComplete(() =>
        {
            
            for (int i = plushieIcons.Count - 1; i >= 0; i--)
            {
                GameObject currentLevelObj = objectivePage.levelDetail[i].levelObject;
                LevelDetail ld = currentLevelObj.GetComponent<LevelDetail>();
                ld.plushieImage.transform.SetParent(ld.transform);
                plushieIcons[i].transform.SetParent(plishieObj[index].transform);
                int levelIndex = PlayerPrefs.GetInt("Level");
                LevelObjectivePageDetail currenPage = MainMenuHandler.instance.levels.levelPage[levelIndex];
                foreach (LevelSelectionObject s in currenPage.levelDetail)
                {
                    s.plushieObject.SetActive(false);
                }
                //plushieIcons[i].transform.localScale = Vector3.one;
                //plushieIcons[i].SetActive(false);
                //plishieObj[index].SetActive(false);
                index++;
                if (index >= plishieObj.Length - 1)
                    index = plishieObj.Length - 1;
            }

        });
       
        //yield return new WaitForSeconds(0.5f);
       
        yield return seq.WaitForCompletion();
        explosionEffectParent.SetActive(true);
      
        ExplodeSound();

        MainMenuHandler.instance.levels.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            foreach (ParticleSystem p in explosionEffect)
            {
                p.Play();
            }
            MainMenuHandler.instance.levels.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                //Invoke(nameof(DisablePlushies), 0.1f);
            });

        });
        yield return new WaitForSeconds(1.5f);
        DisablePlushies();
        ResetScreen();
        //Invoke(nameof(ResetScreen), 2.5f);
    }

    void DisablePlushies()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        LevelObjectivePageDetail currenPage = MainMenuHandler.instance.levels.levelPage[levelIndex];
        foreach (LevelSelectionObject s in currenPage.levelDetail)
        {
            s.plushieObject.SetActive(true);
        }
        foreach (GameObject g in plishieObj)
        {
            g.transform.localScale = Vector3.zero;
            g.SetActive(false);
        }
        levelUpScreen.levelUpIntroScreen.SetActive(false);

        CancelInvoke(nameof(DisablePlushies));
    }
    void EnableInSequence()
    {
     
        if (index >= plishieObj.Length)
        {
            this.GetComponent<Image>().enabled = false;
            StartCoroutine(PlushieSequence());
            return;
        }
    
        currentPlushie = plishieObj[index];
        currentPlushie.SetActive(true);
        if (index > 0)
            plishieObj[index - 1].GetComponentInChildren<LevelUpPlushieInfo>().effect.SetActive(false);
        LevelUpPlushieInfo plushie = currentPlushie.GetComponentInChildren<LevelUpPlushieInfo>();

        currentTween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(plushie.transform, new Vector3(0.5f,0.5f,0.5f), speed, Ease.Linear);
        if (currentTween != null)
        {
            currentTween.OnComplete(() =>
            {
                currentTween.Kill();
                currentTween = null;
                currentTween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(plushie.transform, plushieTargetScaleIn, speed, Ease.Linear);
                CoinBagExplodeSound();
                plushie.effect.SetActive(true);
                plushie.effect.GetComponent<ParticleSystem>().Play();
                plushie.PlaySound();
                if (currentTween != null)
                {
                    currentTween.OnComplete(() =>
                    {

                        currentTween.Kill();
                        currentTween = null;
                        
                        //Debug.LogError("sound");

                        plushie.transform.DOScale(Vector3.one, speed).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            //plushie.effect.SetActive(false);
                            index++;
                            EnableInSequence();
                        });

                    });
                }
            });
        }
        
    }
   
}
