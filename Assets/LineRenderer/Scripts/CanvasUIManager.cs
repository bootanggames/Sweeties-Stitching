using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasUIManager : MonoBehaviour, ICanvasUIManager
{
    [field: SerializeField] public GameObject completeStitchedPlushie {  get; private set; }
    [field: SerializeField] public GameObject gameCompletePanel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI stitchCountText {  get; private set; }
    [field: SerializeField] public TextMeshProUGUI stitchProgress {  get; private set; }
     public GameObject tapToStartButton { get; private set; }
     public TextMeshProUGUI offsetValue { get; private set; }
    [field: SerializeField] public GameObject startText { get; private set; }
    [field: SerializeField] public GameObject undoHighLight { get; private set; }
    [field: SerializeField] public GameObject sewnScreen { get; private set; }
    [field: SerializeField] public GameObject confettiEffectCanvas { get; private set; }
    [field: SerializeField] public GameObject sewnTextImage { get; private set; }
    [field: SerializeField] public GameObject goToHomeScreen { get; private set; }
    [field: SerializeField] public AudioSource audioSourceForBG { get; private set; }
    [field: SerializeField] public Image spoolImg { get; private set; }
    [field: SerializeField] public GameObject mainCanvas { get; private set; }

    private void Start()
    {
        RegisterService();
        //startText.SetActive(true);
        SoundManager.instance.PlaySound(audioSourceForBG, SoundManager.instance.audioClips.bgMusic, true, false, 0.8f, true);
    }
    private void OnDisable()
    {
        UnRegisterService();
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
        LevelsHandler.instance.currentLevelMeta.StartLevel();
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
        stitchProgress.text = Mathf.FloorToInt(percent).ToString() + "% DONE";
    }

    public void CheckGameCompleteOnHomeButton()
    {
        if (!GameHandler.instance.gameStates.Equals(GameStates.Gamecomplete))
        {
            goToHomeScreen.SetActive(true);
            Time.timeScale = 0;
        }
            
    }

    public void TimeScale()
    {
        Time.timeScale = 1;
    }
}
