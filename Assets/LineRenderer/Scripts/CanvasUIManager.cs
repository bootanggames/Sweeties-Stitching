using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasUIManager : MonoBehaviour, ICanvasUIManager
{
    [field: SerializeField] public Slider pullSpeedSlider {  get; private set; }
    [field: SerializeField] public Slider threadCountControlSlider {  get; private set; }
    [field: SerializeField] public Slider detectionRadiusSlider { get; private set; }
    [field: SerializeField] public GameObject completeStitchedPlushie {  get; private set; }
    [field: SerializeField] public GameObject gameCompletePanel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI stitchCountText {  get; private set; }
    [field: SerializeField] public TextMeshProUGUI stitchProgress {  get; private set; }

    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    private void Start()
    {
        UpdateSliderMinMaxValue();
        UpdateThreadMinMaxCount();
        UpdateDetectionRadiusMinMaxValue();
    }
    void UpdateSliderMinMaxValue()
    {
        if (pullSpeedSlider == null) return;
        var pullSpeedHandler = ServiceLocator.GetService<IPointConnectionHandler>();

        if (pullSpeedHandler != null)
        {
            float max = pullSpeedHandler.maxPullDuration;
            pullSpeedSlider.maxValue = max;
            float min = pullSpeedHandler.minPullDuration;
            pullSpeedSlider.minValue = min;
        }
     
    }
    public void OnSpeedValueChange()
    {
        GameEvents.PointConnectionHandlerEvents.onUpdatingPullSpeed.RaiseEvent(pullSpeedSlider.value);
    }

    void UpdateThreadMinMaxCount()
    {
        if (threadCountControlSlider == null) return;

        var threadStitchCountHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if(threadStitchCountHandler != null)
        {
            float maxCount = threadStitchCountHandler.maxThreadStitchCount;
            threadCountControlSlider.maxValue = maxCount;
            float minCount = threadStitchCountHandler.minThreadStitchCount;
            threadCountControlSlider.minValue = minCount;
        }
    }
 
    void UpdateDetectionRadiusMinMaxValue()
    {
        if (detectionRadiusSlider == null) return;

        var needleDetector = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetector != null)
        {
            float minVal = needleDetector.minDetectionRadius;
            detectionRadiusSlider.minValue = minVal;
            float maxVal = needleDetector.maxDetectionRadius;
            detectionRadiusSlider.maxValue = maxVal;
        }
    }
    public void OnThreadStitchValueChange()
    {
        GameEvents.PointConnectionHandlerEvents.onUpdatingStitchCount.RaiseEvent((int)threadCountControlSlider.value);
    }
    public void OnClick(bool value)
    {
        GameEvents.ThreadEvents.setThreadInput.RaiseEvent(value);
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<ICanvasUIManager>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ICanvasUIManager>(this);
    }

    public void SetFreeFormThreadValue(bool value)
    {
        GameEvents.ThreadEvents.onSetFreeformMovementValue.RaiseEvent(value);
    }

    public void UpdateDetectionRadiusValue()
    {
        GameEvents.NeedleDetectorEvents.onSetRadiusValue.RaiseEvent(detectionRadiusSlider.value);
    }

    public void SetLevel2()
    {
        LevelsHandler.instance.SetPref(1);
        GameHandler.instance.Retry();
    }
    public void SetLevel1()
    {
        LevelsHandler.instance.SetPref(0);
        GameHandler.instance.Retry();
    }
    public void TapToStart()
    {
        GameEvents.ThreadEvents.setThreadInput.RaiseEvent(true);
        GameHandler.instance.SwitchGameState(GameStates.Gamestart);
        var levelhandler = ServiceLocator.GetService<ILevelHandler>();
        if(levelhandler != null)
        {
            levelhandler.levels[levelhandler.levelIndex].GetComponent<Level_Metadata>().StartLevel();
        }
    }

    public void UpdateStitchCount(int totalStitch, int completedStitch)
    {
        stitchCountText.text = completedStitch + " OF " + totalStitch;
    }

    public void UpdatePlushieStitchProgress(int totalParts, int completedParts)
    {
        float percent = 0;
        if (totalParts == 0)
            percent = 0;
        else
            percent = ((float)completedParts / totalParts) * 100;
        stitchProgress.text = Mathf.FloorToInt(percent).ToString() + "% DONE ";

    }
}
