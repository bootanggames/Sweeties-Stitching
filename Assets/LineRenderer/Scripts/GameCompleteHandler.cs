using DG.Tweening;
using UnityEngine;

public class GameCompleteHandler : MonoBehaviour, IGameService
{
    [SerializeField] float speed;
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
    }

    public void UnRegisterService()
    {
        GameEvents.GameCompleteEvents.onGameComplete.UnregisterEvent(GameComplete);
    }

    void GameComplete()
    {
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
    }
}
