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
    [SerializeField] GameObject[] sparkles;
    [SerializeField] int sparkleIndex = 0;
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
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
    void WinConfettiEffect()
    {
        PlaySound();
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


    void GameComplete()
    {
        if(sparkleRoutine == null)
            sparkleRoutine = StartCoroutine(EnableSparkle());
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
            canvasHandler.gameCompletePanel.gameObject.SetActive(true);
            canvasHandler.completeStitchedPlushie.SetActive(true);
            RectTransform rt =  canvasHandler.completeStitchedPlushie.GetComponent<RectTransform>();
            rt.DOScale(1, speed).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (coinsHandler != null)
                {
                    coinsHandler.CreateCoinsObjects();
                    StartCoroutine(coinsHandler.MoveCoins());
                }
            });
        }
        GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
        
    }
    Coroutine sparkleRoutine = null;
    public float speedSparkle;  
   
    IEnumerator EnableSparkle()
    {
        while (true)
        {
            var sparkle = sparkles[sparkleIndex];
            CanvasGroup cg = sparkle.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            sparkle.SetActive(true);

            yield return cg.DOFade(1f, speedSparkle * 0.5f)
                .SetEase(Ease.OutQuad)
                .WaitForCompletion();

            yield return cg.DOFade(0f, speedSparkle * 0.5f)
                .SetEase(Ease.InQuad)
                .WaitForCompletion();

            sparkleIndex = (sparkleIndex + 1) % sparkles.Length;

            yield return new WaitForSeconds(Random.Range(0.001f, 0.01f));
        }
    }
}
