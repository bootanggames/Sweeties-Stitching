using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICanvasUIManager:IGameService
{
     Slider pullSpeedSlider { get; }
     Slider threadCountControlSlider {  get; }
     Slider detectionRadiusSlider {  get; }
     Slider needleOffset {  get; }
    GameObject completeStitchedPlushie { get; }
    GameObject gameCompletePanel { get; }
    GameObject tapToStartButton { get; }
    TextMeshProUGUI stitchCountText {  get; }
    TextMeshProUGUI stitchProgress {  get; }
    TextMeshProUGUI offsetValue {  get; }
    void TapToStart();
    void UpdateStitchCount(int totalStitch, int completedStitch);
    void UpdatePlushieStitchProgress(int totalParts, int completedParts);
}
