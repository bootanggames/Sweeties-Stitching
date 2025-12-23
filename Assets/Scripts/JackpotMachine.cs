using UnityEngine;

public class JackpotMachine : MonoBehaviour
{
    [field: SerializeField] public GameObject jackpotWord { get; private set; }
    [field:SerializeField]  public GameObject jackPotMachineObject {  get; private set; }
    [field: SerializeField] public GameObject congratulationsScreen { get; private set; }
    [field: SerializeField] public GameObject UIObject { get; private set; }
    [field: SerializeField] public RewardItem rewardedItem { get; private set; }
    [field: SerializeField] public ItemRepository itemRepository { get; private set; }
    IJackpotHandler jackpotHandler;
    private void Start()
    {
        HepticManager.instance.StopHaptics();
        jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
        jackpotWord.SetActive(true);
    }
    public void CloseScreen()
    {
        jackpotHandler.CloseJackpotScreen();
    }
    public void ShowRewardScreen()
    {
        //Debug.LogError("reward screen");
        UIObject.SetActive(false);
        jackPotMachineObject.SetActive(false);
        congratulationsScreen.gameObject.SetActive(true);
        Invoke(nameof(CloseRewardedScreen), 1.0f);
    }
    public void CloseRewardedScreen()
    {
        UIObject.SetActive(true);
        jackPotMachineObject.SetActive(true);
        jackpotHandler.CloseRewardScreen();
        CancelInvoke(nameof(CloseRewardedScreen));
    }
    public void EnableCongratulationsScreen(bool val)
    {
        congratulationsScreen.SetActive(val);
        jackPotMachineObject.SetActive(!val);
    }
}
