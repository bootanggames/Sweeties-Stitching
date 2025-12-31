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
    [field: SerializeField] public JackpotRewardSystem jackPotRewardsystem {  get; private set; }
    private void OnEnable()
    {
        jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
    }
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
    public void ShowRewardScreen(RewardType type)
    {
        //Debug.LogError("reward screen");
        if(UIObject)
            UIObject.SetActive(false);
        jackPotMachineObject.SetActive(false);
        congratulationsScreen.gameObject.SetActive(true);
        UpdateRewardItemScreen(type);
        //Invoke(nameof(CloseRewardedScreen), 1.0f);
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

    public void UpdateRewardItemScreen(RewardType type)
    {
        JackpotReward rewardItem = jackPotRewardsystem.jackPotRewardsScriptable.GetRewardItem(type);
        ItemsMetaData item = rewardItem.GetItem();
        rewardedItem.imageComponent.sprite = item.ItemIcon;
        rewardedItem.rewardAmountText.text = rewardItem.rewardAmount.ToString();
        if(!jackpotHandler.earnedItems.Contains(item))
            jackpotHandler.earnedItems.Add(item);
    }
}
