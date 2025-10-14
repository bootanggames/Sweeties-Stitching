using DG.Tweening;
using UnityEngine;

public class GameCompleteHandler : MonoBehaviour, IGameService
{
    [SerializeField] float speed;
    [SerializeField] ParticleSystem[] confettiEffect;
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
        foreach (ParticleSystem ps in  confettiEffect)
        {
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }
    void GameComplete()
    {
        GameEvents.ThreadEvents.setThreadInput.RaiseEvent(false);
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if(canvasHandler != null)
        {
            canvasHandler.gameCompletePanel.gameObject.SetActive(true);
            canvasHandler.completeStitchedPlushie.SetActive(true);
            RectTransform rt =  canvasHandler.completeStitchedPlushie.GetComponent<RectTransform>();
            rt.DOScale(1, speed).SetEase(Ease.Linear).OnComplete(() =>
            {

            });
        }
        GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
        
    }
}
