using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackpotMachine : MonoBehaviour
{
    [field: SerializeField] public GameObject jackpotWord { get; private set; }
    [field:SerializeField]  public GameObject jackPotMachineObject {  get; private set; }
    [field: SerializeField] public GameObject congratulationsScreen { get; private set; }
    [field: SerializeField] public GameObject UIObject { get; private set; }
    [field: SerializeField] public GameObject AddToCollectionButton { get; private set; }
    [field: SerializeField] public RewardItem rewardedItem { get; private set; }
    [field: SerializeField] public ItemRepository itemRepository { get; private set; }
    IJackpotHandler jackpotHandler;
    [field: SerializeField] public JackpotRewardSystem jackPotRewardsystem {  get; private set; }
    [field: SerializeField] public GameObject jackpotCamera { get; private set; }

    [SerializeField] GameObject jackpotHandle;
    [SerializeField] GameObject jackpotHandleCircle;
    [SerializeField] Vector3 handleStartPosition;
    [SerializeField] Vector3 handleEndPosition;
    [SerializeField] AudioSource source;
    [SerializeField] Sprite coinIcon;
    [SerializeField] List<GameObject> lightBulb;
    [SerializeField] GameObject winText;
    int bulbIndex = 0;
    IRewardSystem rewardSystem;
    private void OnEnable()
    {
        jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
        rewardSystem = ServiceLocator.GetService<IRewardSystem>();
    }
    private void Start()
    {
        HepticManager.instance.StopHaptics();
        jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
        jackpotWord.SetActive(true);
        Invoke(nameof(JackPotScreenSound), 0.15f);
    }
    void JackPotScreenSound()
    {
        AudioClip clip = SoundManager.instance.audioClips.youHaveJackpot;
        SoundManager.instance.PlaySound(source, clip, false, false, 1, false);
        CancelInvoke(nameof(JackPotScreenSound));
    }
    void JackPotRewardScreenSound()
    {
        AudioClip clip = SoundManager.instance.audioClips.jackpotPrize;
        SoundManager.instance.PlaySound(source, clip, false, false, 1, false);
        CancelInvoke(nameof(JackPotScreenSound));
    }
    public void CloseScreen()
    {
        jackpotHandler.CloseJackpotScreen();
    }
    public void ShowHugeRewardScreen(RewardType type)
    {
        ShowRewardScreen();
        UpdateRewardItemScreen(type);

        //Invoke(nameof(CloseRewardedScreen), 1.0f);
    }
    public void ShowRewardScreen()
    {
        if (UIObject)
            UIObject.SetActive(false);
        jackPotMachineObject.SetActive(false);
        congratulationsScreen.gameObject.SetActive(true);
        JackPotRewardScreenSound();
    }
    public void ShowSmallReward(double rewardAmount)
    {
        ShowRewardScreen();
        rewardedItem.imageComponent.sprite = coinIcon;
        rewardAmount = rewardSystem.levelUpRewardHandler.EVLevelJackpot_CoinsOnly();
        rewardedItem.rewardAmountText.text = int.Parse(rewardAmount).ToString();
    }
    public void ShowMediumReward(int rewardAmount)
    {
        ShowRewardScreen();
        rewardedItem.imageComponent.sprite = coinIcon;
        rewardedItem.rewardAmountText.text = rewardAmount.ToString();
    }
    public void EnableRoomBg(bool val)
    {
        jackpotCamera.SetActive(!val);
        jackpotHandler.mainCamera.SetActive(val);
        jackpotHandler.roomBg.SetActive(val);
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
        if (!jackpotHandler.earnedItems.Contains(item))
                jackpotHandler.earnedItems.Add(item);
    }

    public void StartSpin()
    {
        jackpotHandle.SetActive(false);
        jackpotHandleCircle.transform.localPosition = handleEndPosition;
        Invoke(nameof(ResetHandle), 1.0f);
    }

    void ResetHandle()
    {
        jackpotHandle.SetActive(true);
        jackpotHandleCircle.transform.localPosition = handleStartPosition;
        CancelInvoke(nameof(ResetHandle));
    }
    public void WinEffect()
    {
        StartCoroutine(ShowBlinkinOfObjects());
    }
    IEnumerator ShowBlinkinOfObjects()
    {
        lightBulb[bulbIndex].gameObject.SetActive(false);
        winText.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        winText.SetActive(true);

        yield return new WaitForSeconds(0.075f);

        foreach (GameObject g in lightBulb)
        {
            g.SetActive(true);
        }

        bulbIndex++;
        if (bulbIndex >= lightBulb.Count)
            bulbIndex = 0;
        StopCoroutine(ShowBlinkinOfObjects());
        StartCoroutine(ShowBlinkinOfObjects());
    }
}
