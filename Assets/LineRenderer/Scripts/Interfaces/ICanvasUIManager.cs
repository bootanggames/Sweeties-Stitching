using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICanvasUIManager:IGameService
{
    GameObject completeStitchedPlushie { get; }
    GameObject gameCompletePanel { get; }
    GameObject goToHomeScreen { get; }
    GameObject tapToStartButton { get; }
    GameObject startText { get; }
    GameObject sewnScreen { get; }
    GameObject sewnTextImage { get; }
    GameObject undoHighLight { get; }
    GameObject confettiEffectCanvas { get; }
    TextMeshProUGUI stitchCountText {  get; }
    TextMeshProUGUI stitchProgress {  get; }
    TextMeshProUGUI offsetValue {  get; }
    void TapToStart();
    void UpdateStitchCount(int totalStitch, int completedStitch);
    void UpdatePlushieStitchProgress(int totalParts, int completedParts);
}
