using UnityEngine;
using UnityEngine.UI;

public interface ICanvasUIManager:IGameService
{
     Slider pullSpeedSlider { get; }
     Slider threadCountControlSlider {  get; }
     Slider detectionRadiusSlider {  get; }
}
