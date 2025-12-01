using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCompleteHandler : MonoBehaviour, IGameService
{
    [SerializeField] float speed;
    [SerializeField] List<GameObject> confettiEffect;
    [SerializeField] Transform[] effectPosition;
    [SerializeField] Image plushieOfCurrentLevel;

    [SerializeField] TextMeshProUGUI levelProgress;
    [SerializeField] GameObject gameplayBgObj;

    [Header("-------------Sparkle Trail On Completion--------------")]
    [SerializeField] Transform sparkleTrailAtCompletionStartPos;
    [SerializeField] Transform sparkleTrailAtCompletionTargetPos;
    [SerializeField] float sparkleMoveSpeed;

    [SerializeField] private GameObject _coinBurstObject;
    [SerializeField] bool test;
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        GameEvents.GameCompleteEvents.onGameComplete.RegisterEvent(GameComplete);
        GameEvents.GameCompleteEvents.onGameWin.RegisterEvent(WinConfettiEffect);
        GameEvents.GameCompleteEvents.onPlushieComplete.RegisterEvent(SparkleEffectOnPlushieComplete);
    }

    public void UnRegisterService()
    {
        GameEvents.GameCompleteEvents.onGameComplete.UnregisterEvent(GameComplete);
        GameEvents.GameCompleteEvents.onGameWin.UnregisterEvent(WinConfettiEffect);
        GameEvents.GameCompleteEvents.onPlushieComplete.UnregisterEvent(SparkleEffectOnPlushieComplete);
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

    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.congratulationsScreenSound;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
    void GameComplete()
    {
        GameEvents.ThreadEvents.setThreadInput.RaiseEvent(false);
        if (LevelsHandler.instance)
        {
            plushieOfCurrentLevel.sprite = LevelsHandler.instance.currentLevelMeta.plushieSprite;
            RectTransform rt = plushieOfCurrentLevel.rectTransform;
            rt.sizeDelta = new Vector2(LevelsHandler.instance.currentLevelMeta.plushieWidth, LevelsHandler.instance.currentLevelMeta.plushieHeight);
            levelProgress.text = (LevelsHandler.instance.levelIndex + 1) + "/3 Till Level 5";
        }
        var plushieInventory = ServiceLocator.GetService<IPlushieStoreHandler>();
        if (plushieInventory != null)
            plushieInventory.GetPlushieCountUI();

        var coinsHandler = ServiceLocator.GetService<ICoinsHandler>();
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if(canvasHandler != null)
        {
            canvasHandler.sewnScreen.SetActive(false);
            canvasHandler.sewnTextImage.transform.localScale = Vector3.zero;
            canvasHandler.confettiEffectCanvas.SetActive(false);
            canvasHandler.mainCanvas.SetActive(false);
            LevelsHandler.instance.currentLevelMeta.sewnPlushie.SetActive(false);
            canvasHandler.gameCompletePanel.gameObject.SetActive(true);
            Invoke(nameof(TreasureBoxAppearance), 0.45f);
            canvasHandler.completeStitchedPlushie.SetActive(true);
            PlaySound();
            RectTransform rt =  canvasHandler.completeStitchedPlushie.GetComponent<RectTransform>();
            rt.DOScale(1, speed).SetEase(Ease.Linear).OnComplete(() =>
            {
                //if (coinsHandler != null)
                //{
                //    coinsHandler.CreateCoinsObjects();
                //    StartCoroutine(coinsHandler.MoveCoins(coinsHandler.coinsObjList, coinsHandler.targetPointToMove, coinsHandler.coinBar, coinsHandler.coinMoveSpeed,Ease.InOutBack,0.01f));
                //}
            });
        }
        GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
    }
   
    void PlaySoundCoinBagExploding()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.coinBagExploding;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
    void TreasureBoxAppearance()
    {
        _coinBurstObject.SetActive(true);
        PlaySoundCoinBagExploding();
        var icoinsHandler = ServiceLocator.GetService<ICoinsHandler>();
        if (icoinsHandler != null)
        {
            icoinsHandler.CoinIncrementAnimation(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelReward);
        }
        gameplayBgObj.SetActive(false);
    }
   
    [SerializeField] List<GameObject> sparkleEffectListOnComplete;
    void SparkleEffectOnPlushieComplete()
    {
        int _count = 10;
        Invoke(nameof(CleanEnablePlushie), 0.5f);
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < _count; i++)
        {
            GameObject g = GameEvents.EffectHandlerEvents.onSparkleTrailEffectOnCompletion.Raise(sparkleTrailAtCompletionStartPos);

            if (!sparkleEffectListOnComplete.Contains(g))
                sparkleEffectListOnComplete.Add(g);
            g.transform.SetParent(gameplayBgObj.transform);
            g.transform.localPosition = sparkleTrailAtCompletionStartPos.localPosition;
            Vector3 startPos = g.transform.position;
            
           
            Vector3 targetPos = sparkleTrailAtCompletionTargetPos.localPosition;
            if (i != 0)
            {
                if (sparkleEffectListOnComplete.Count % 2 == 0)
                {
                    startPos.y += 2f;
                    targetPos.y += 2f;
                }
                else
                {
                    startPos.y -= 2f;
                    targetPos.y -= 2f;

                }
            }
            float distance = Vector3.Distance(startPos, targetPos);

            float baseSpeed = sparkleMoveSpeed;

            float speedVariation = Random.Range(0.4f, 0.5f); 
            float speed = baseSpeed * speedVariation;

            float duration = distance / speed;
            //if(sparkleEffectListOnComplete.Count % 2 == 0)
            //{
            //    Vector3 pos = sparkleEffectListOnComplete[i].transform.localPosition;
            //    pos.y += 10;
            //    sparkleEffectListOnComplete[i].transform.localPosition = pos;
            //}
            seq.Join( GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.Raise(g.transform, targetPos, duration, Ease.Linear));
            seq.Join(GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(g.transform, Vector3.zero, 3.5f, Ease.Linear));

            seq.OnComplete(() =>
            {
                sparkleEffectListOnComplete.Remove(g);
                WinEffect();
                //Invoke(nameof(WinEffect), 0.5f);
                Destroy(g);
            });
        }
       
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
    void WinEffect()
    {
        Time.timeScale = 1.0f;

        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
            canvasHandler.sewnScreen.SetActive(true);
        CancelInvoke(nameof( WinEffect));
    }
}
