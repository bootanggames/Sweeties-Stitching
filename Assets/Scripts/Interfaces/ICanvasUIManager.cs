using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICanvasUIManager:IGameService
{
    GameObject completeStitchedPlushie { get; }
    GameObject tapToStartButton { get; }
    GameObject startText { get; }
    GameObject sewnTextImage { get; }
    GameObject undoHighLight { get; }
    GameObject alertTextObj { get; }
    TextMeshProUGUI stitchCountText {  get; }
    TextMeshProUGUI stitchProgress {  get; }
    TextMeshProUGUI offsetValue {  get; }
    AudioSource audioSourceForBG {  get; }
    GameplayScreens screens{ get; }
    void TapToStart();
    void UpdateStitchCount(int totalStitch, int completedStitch);
    void UpdatePlushieStitchProgress(int totalParts, int completedParts);
    void PlayBgMusic();
    void StopBgMusic();
    void EnableDisablePlushieInventoryScreen(bool active);
    void EnableDisableGameCompleteScreen(bool active);
}
