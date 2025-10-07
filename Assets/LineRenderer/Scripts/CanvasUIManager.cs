using UnityEngine;
using UnityEngine.UI;

public class CanvasUIManager : MonoBehaviour
{
    [SerializeField] Slider pullSpeedSlider;
    [SerializeField] Slider threadCountControlSlider;

    private void Start()
    {
        UpdateSliderMaxValue();
    }
    void UpdateSliderMaxValue()
    {
        float max = GameEvents.PointConnectionHandlerEvents.onGettingMaxSpeed.RaiseEvent();
        pullSpeedSlider.maxValue = max;
    }
    public void OnValueChange()
    {
        GameEvents.PointConnectionHandlerEvents.onUpdatingPullSpeed.RaiseEvent(pullSpeedSlider.value);
    }

}
