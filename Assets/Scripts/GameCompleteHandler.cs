using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCompleteHandler : MonoBehaviour, IGameService
{
    [SerializeField] float speed;
    /*[SerializeField]*/ List<GameObject> confettiEffect;
    [SerializeField] Transform[] effectPosition;
    [SerializeField] Image plushieOfCurrentLevel;

    [SerializeField] TextMeshProUGUI levelProgress;
    [SerializeField] GameObject gameplayBgObj;
    [SerializeField] AudioSource _audioSource;

    [Header("-------------Sparkle Trail On Completion--------------")]
    [SerializeField] Transform sparkleTrailAtCompletionStartPos;
    [SerializeField] Transform sparkleTrailAtCompletionTargetPos;
    [SerializeField] float sparkleMoveSpeed;
    /*[SerializeField]*/
    List<GameObject> sparkleEffectListOnComplete = new List<GameObject>();

    [SerializeField] private GameObject _coinBurstParentObject;
    [SerializeField] private GameObject[] _coinBurstObject;
    ICoinsHandler coinsHandler;
    ICanvasUIManager canvasHandler;
    IPlushieStoreHandler plushieInventory;
    //IJackpotHandler jackpotHandler;
    private void OnEnable()
    {
        RegisterService();
        coinsHandler = ServiceLocator.GetService<ICoinsHandler>();
        canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        plushieInventory = ServiceLocator.GetService<IPlushieStoreHandler>();
        //jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
    }
    private void Start()
    {
        coinsHandler = ServiceLocator.GetService<ICoinsHandler>();
        canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        plushieInventory = ServiceLocator.GetService<IPlushieStoreHandler>();
        //jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        GameEvents.GameCompleteEvents.onGameComplete.Register(GameComplete);
        GameEvents.GameCompleteEvents.onGameWin.Register(WinConfettiEffect);
        GameEvents.GameCompleteEvents.onPlushieComplete.Register(SparkleEffectOnPlushieComplete);
        GameEvents.GameCompleteEvents.onFinishCoinBurstAnimation.Register(StopCoinBurstSound);
        
    }

    public void UnRegisterService()
    {
        GameEvents.GameCompleteEvents.onGameComplete.UnRegister(GameComplete);
        GameEvents.GameCompleteEvents.onGameWin.UnRegister(WinConfettiEffect);
        GameEvents.GameCompleteEvents.onPlushieComplete.UnRegister(SparkleEffectOnPlushieComplete);
        GameEvents.GameCompleteEvents.onFinishCoinBurstAnimation.UnRegister(StopCoinBurstSound);
    }

    void WinConfettiEffect()
    {
        foreach (Transform ps in  effectPosition)
        {
            GameObject effect = GameEvents.EffectHandlerEvents.onGetInstantiatedEffect.Raise(ps);
            confettiEffect.Add(effect);
        }
        foreach(GameObject effect in confettiEffect)
        {
            effect.SetActive(true);
            if (effect.GetComponent<ParticleSystem>()) effect.GetComponent<ParticleSystem>().Play();
        }
    }

    void PlayGiggleSound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.congratulationsScreenSound;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }

    void GameComplete()
    {
        GameEvents.ThreadEvents.setThreadInput.Raise(false);
        if (LevelsHandler.instance)
        {
            plushieOfCurrentLevel.sprite = LevelsHandler.instance.currentLevelMeta.levelScriptable.plushieSprite;
            RectTransform rt = plushieOfCurrentLevel.rectTransform;
            rt.sizeDelta = new Vector2(LevelsHandler.instance.currentLevelMeta.levelScriptable.plushieWidth, LevelsHandler.instance.currentLevelMeta.levelScriptable.plushieHeight);
            int nextLevelIndex = LevelsHandler.instance.levelIndex + 2;
            if (nextLevelIndex > LevelsHandler.instance.levelStructure.Count)
                nextLevelIndex = LevelsHandler.instance.levelStructure.Count;
            levelProgress.text = (LevelsHandler.instance.plushieIndex + 1) + "/3 Till Level "+ nextLevelIndex;
        }
        if (plushieInventory != null)
            plushieInventory.GetPlushieCountUI();

        if(canvasHandler != null)
        {
            PlayCelebrationTrumpetSound();
            canvasHandler.sewnScreen.SetActive(false);
            canvasHandler.sewnTextImage.transform.localScale = Vector3.zero;
            canvasHandler.confettiEffectCanvas.SetActive(false);
            canvasHandler.mainCanvas.SetActive(false);
            canvasHandler.audioSourceForBG.volume = 0.5f;
            LevelsHandler.instance.currentLevelMeta.sewnPlushie.SetActive(false);
            canvasHandler.EnableDisableGameCompleteScreen(true);
 
            Invoke(nameof( PlaySparkleSound),0.15f);
            Invoke(nameof(TreasureBoxAppearance), 0.45f);
            canvasHandler.completeStitchedPlushie.SetActive(true);
            PlayGiggleSound();
            RectTransform rt =  canvasHandler.completeStitchedPlushie.GetComponent<RectTransform>();
            rt.DOScale(1, speed).SetEase(Ease.Linear).OnComplete(() =>
            {
                canvasHandler.PlayBgMusic();
                GameHandler.instance.SwitchGameState(GameStates.jackpotScreen);
                //if (coinsHandler != null)
                //{
                //    coinsHandler.CreateCoinsObjects();
                //    StartCoroutine(coinsHandler.MoveCoins(coinsHandler.coinsObjList, coinsHandler.targetPointToMove, coinsHandler.coinBar, coinsHandler.coinMoveSpeed,Ease.InOutBack,0.01f));
                //}
            });
        }
        GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
            
    }
    void EnhanceGiggleSoundVolume()
    {
        SoundManager.instance.audioSource.volume = 1;
        //jackpotHandler.ShowJackPotScreen();
    }
    void PlayCelebrationTrumpetSound()
    {
        AudioClip _clip = SoundManager.instance.audioClips.celebrationJingleTrumpets;
        SoundManager.instance.PlaySound(_audioSource, _clip, false, false, 0.5f, false);
    }
    void PlaySparkleSound()
    {
        canvasHandler.gameCompletePanel.gameObject.AddComponent<AudioSource>();
        AudioSource source = canvasHandler.gameCompletePanel.gameObject.GetComponent<AudioSource>();
        AudioClip _clip = SoundManager.instance.audioClips.plushieCompletedSparkle;
        SoundManager.instance.PlaySound(source, _clip, false, false, 1, false);
        CancelInvoke(nameof(PlaySparkleEffect));
    }
    public void StopCoinBurstSound()
    {
        AudioSource source = _coinBurstParentObject.GetComponent<AudioSource>();
        SoundManager.instance.StopSound(source);
    }
    void PlaySoundCoinBagExploding()
    {
        //SoundManager.instance.ResetAudioSource();
        SoundManager.instance.audioSource.volume = 1.0f;
        _coinBurstParentObject.AddComponent<AudioSource>();
        AudioSource source = _coinBurstParentObject.GetComponent<AudioSource>();
        AudioClip _clip = SoundManager.instance.audioClips.coinBagExploding;
        SoundManager.instance.PlaySound(source, _clip, false, false, 1, false);
        Invoke(nameof(EnhanceGiggleSoundVolume), 0.25f);
    }
    void TreasureBoxAppearance()
    {
        foreach(GameObject g in _coinBurstObject)
        {
            g.SetActive(true);
        }
        PlaySoundCoinBagExploding();
        if (coinsHandler != null)
            coinsHandler.CoinIncrementAnimation(LevelsHandler.instance.currentLevelMeta.levelReward);
            //coinsHandler.CoinIncrementAnimation(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelReward);
        gameplayBgObj.SetActive(false);
    }
   
    void SparkleEffectOnPlushieComplete()
    {
        //int _count = 10;
        PlaySparkleEffect();
        GameEvents.EffectHandlerEvents.onSewnCompletely.Raise();

        Invoke(nameof(CleanEnablePlushie), 0.5f);
        Sequence seq = DOTween.Sequence();
        //for (int i = 0; i < _count; i++)
        {
            GameObject g = GameEvents.EffectHandlerEvents.onSparkleTrailEffectOnCompletion.Raise(sparkleTrailAtCompletionStartPos);

            if (!sparkleEffectListOnComplete.Contains(g))
                sparkleEffectListOnComplete.Add(g);
            g.transform.SetParent(gameplayBgObj.transform);
            g.transform.localPosition = sparkleTrailAtCompletionStartPos.localPosition;
            Vector3 startPos = g.transform.position;
            Vector3 targetPos = sparkleTrailAtCompletionTargetPos.localPosition;

            //if (i != 0)
            //{
            //    if (sparkleEffectListOnComplete.Count % 2 == 0)
            //    {
            //        startPos.y += 2f;
            //        targetPos.y += 2f;
            //    }
            //    else
            //    {
            //        startPos.y -= 2f;
            //        targetPos.y -= 2f;

            //    }
            //}
            float distance = Vector3.Distance(startPos, targetPos);

            float baseSpeed = sparkleMoveSpeed;

            float speedVariation = Random.Range(0.4f, 0.5f); 
            float speed = baseSpeed * speedVariation;

            float duration = distance / speed;
          
            seq.Join( GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.Raise(g.transform, targetPos, duration, Ease.Linear));
            seq.Join(GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(g.transform, Vector3.zero, 3.5f, Ease.Linear));

            seq.OnComplete(() =>
            {
                sparkleEffectListOnComplete.Remove(g);
                Destroy(g);
            });
        }
        EnableSewnWord();
    }
    void PlaySparkleEffect()
    {
        SoundManager.instance.ResetAudioSource();

        AudioClip _clip = SoundManager.instance.audioClips.sparklesSound;
        SoundManager.instance.PlaySound(_audioSource, _clip, false, false, 0.5f, false);
    }
    void CleanEnablePlushie()
    {
        foreach (Connections c in LevelsHandler.instance.currentLevelMeta.cleanConnection)
        {
            Destroy(c.line.gameObject);
        }
        LevelsHandler.instance.currentLevelMeta.cleanConnection.Clear();
        foreach (GameObject g in LevelsHandler.instance.currentLevelMeta.crissCrossObjList)
        {
            Destroy(g);
        }
        LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Clear();
        CancelInvoke(nameof(CleanEnablePlushie));
    }
   
    void EnableSewnWord()
    {
        Time.timeScale = 1.0f;
        if (canvasHandler != null)
        {
            canvasHandler.confettiEffectCanvas.SetActive(true);
            canvasHandler.sewnScreen.SetActive(true);
        }
        CancelInvoke(nameof( EnableSewnWord));
    }

  
}
