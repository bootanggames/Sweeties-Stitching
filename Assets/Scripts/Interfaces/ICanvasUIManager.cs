using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICanvasUIManager:IGameService
{
    GameObject tapToStartButton { get; }
    TextMeshProUGUI offsetValue {  get; }
    GameplayUIData uiData { get; }
    GameplayScreens screens{ get; }
    void TapToStart();
    void UpdateStitchCount(int totalStitch, int completedStitch);
    void UpdatePlushieStitchProgress(int totalParts, int completedParts);
    void PlayBgMusic();
    void StopBgMusic();
    void EnableDisablePlushieInventoryScreen(bool active);
    void EnableDisableGameCompleteScreen(bool active);
}
