using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICanvasUIManager:IGameService
{
     Slider pullSpeedSlider { get; }
     Slider threadCountControlSlider {  get; }
     Slider detectionRadiusSlider {  get; }
    GameObject completeStitchedPlushie { get; }
    GameObject gameCompletePanel { get; }
    TextMeshProUGUI stitchCount {  get; }
    TextMeshProUGUI stitchProgress {  get; }
    void UpdateStitchCount(int total, int noOfStitchesDone);
    void UpdateStitchProgress(int totalParts, int noOfPartDone);
}
