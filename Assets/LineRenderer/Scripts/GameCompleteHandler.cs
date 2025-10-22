using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCompleteHandler : MonoBehaviour, IGameService
{
    [SerializeField] float speed;
    [SerializeField] List<GameObject> confettiEffect;
    [SerializeField] Transform[] effectPosition;
    [SerializeField] Image plushieOfCurrentLevel;
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
        var pointHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if (pointHandler != null)
        {
            foreach (Connections c in pointHandler.connections)
            {
                Destroy(c.line.gameObject);
            }
            pointHandler.connections.Clear();
        }
    }
    void GameComplete()
    {
        GameEvents.ThreadEvents.setThreadInput.RaiseEvent(false);

        plushieOfCurrentLevel.sprite = LevelsHandler.instance.currentLevelMeta.plushieSprite;
        var coinsHandler = ServiceLocator.GetService<ICoinsHandler>();
 
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if(canvasHandler != null)
        {
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
}
