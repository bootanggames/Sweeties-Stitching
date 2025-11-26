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
    [SerializeField] GameObject chestObj;
    [SerializeField] GameObject chestSmokeEffect;
    [SerializeField] GameObject chestTarget;
    [SerializeField] GameObject chestTop;
    [SerializeField] GameObject gameplayBgObj;
    [SerializeField] float treasureMoveSpeed;
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
    }

    public void UnRegisterService()
    {
        GameEvents.GameCompleteEvents.onGameComplete.UnregisterEvent(GameComplete);
        GameEvents.GameCompleteEvents.onGameWin.UnregisterEvent(WinConfettiEffect);
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
            Invoke(nameof(TreasureBoxAppearance), 0.2f);
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

    void TreasureBoxAppearance()
    {
        gameplayBgObj.SetActive(false);
        chestObj.transform.DOLocalMove(chestTarget.transform.localPosition, treasureMoveSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            chestSmokeEffect.gameObject.SetActive(true);
            chestSmokeEffect.GetComponent<ParticleSystem>().Play();
            chestTop.GetComponent<ChestTopRotation>().enabled = true;
            
        });
        CancelInvoke(nameof(treasureMoveSpeed));
    }
}
