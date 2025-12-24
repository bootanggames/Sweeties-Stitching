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
            //foreach (GameObject g in plishieObj)
            //{
            //    LevelUpPlushieInfo plushie = g.GetComponentInChildren<LevelUpPlushieInfo>();
            //    plushie.effect.SetActive(false);
            //    plushie.transform.localScale = Vector3.zero;
            //    g.SetActive(false);
            //}
            jackpotHandler.ShowJackPotScreen();
            index = 0;
        }
    }

    void CoinBagExplodeSound()
    {
        SoundManager.instance.PlaySound(source, SoundManager.instance.audioClips.coinBagExploding, false, false, 1, false);
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
            seq.Join(GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(plushieIcon.transform, new Vector3(0.75f, 0.75f, 0.75f), speed, Ease.Linear));
        }
        yield return new WaitForSeconds(0.2f);

        foreach (GameObject g in plishieObj)
        {
            g.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);

        seq.OnComplete(() =>
        {
           
            for (int i = plushieIcons.Count - 1; i >= 0; i--)
            {
                GameObject currentLevelObj = objectivePage.levelDetail[i].levelObject;
                LevelDetail ld = currentLevelObj.GetComponent<LevelDetail>();
                ld.plushieImage.transform.SetParent(ld.transform);
                plushieIcons[i].transform.SetParent(plishieObj[index].transform);
                plushieIcons[i].transform.localScale = Vector3.zero;
                //plushieIcons[i].SetActive(false);
                //plishieObj[index].SetActive(false);
                index++;
                if (index >= plishieObj.Length - 1)
                    index = plishieObj.Length - 1;
            }
        });
        yield return seq.WaitForCompletion();
        ResetScreen();
        //Invoke(nameof(ResetScreen), 2.5f);
    }
    void EnableInSequence()
    {
     
        if (index >= plishieObj.Length)
        {
            StartCoroutine(PlushieSequence());
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
