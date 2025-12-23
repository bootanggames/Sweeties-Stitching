using UnityEngine;

public class JackpotHandler : MonoBehaviour, IJackpotHandler
{
    [field: SerializeField] public GameObject jackpotPrefab {  get; private set; }
    [field: SerializeField] public GameObject gameplayCamera {  get; private set; }
    [field: SerializeField] public GameObject gameplayBg {  get; private set; }
    [field: SerializeField] public GameObject gameplayCanvas {  get; private set; }
    [field: SerializeField] public GameObject gameCompleteCanvas {  get; private set; }
    [field: SerializeField] public GameObject plushieInventoryCanvas {  get; private set; }

    GameObject jackpotScreen = null;
    ICoinsHandler coinsHandler;
    private void OnEnable()
    {
        RegisterService();
    }
    private void Start()
    {
        coinsHandler = ServiceLocator.GetService<ICoinsHandler>();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void ShowJackPotScreen()
    {
        Invoke(nameof(ShowWithDelay), 0.5f);
    }
    void ShowWithDelay()
    {
        jackpotScreen = Instantiate(jackpotPrefab);
        jackpotScreen.SetActive(true);
        gameplayCamera.SetActive(false);
        gameplayCanvas.SetActive(false);
        gameCompleteCanvas.SetActive(false);
        plushieInventoryCanvas.SetActive(false);
        MainMenuHandler.instance.mainMenuBg.SetActive(false);
        CancelInvoke(nameof(ShowWithDelay));
        //CancelInvoke(nameof(coinsHandler.PlayCoinSoundOnComplete));
        //SoundManager.instance.StopSound(coinsHandler.audioSource);
        //coinsHandler.audioSource.enabled = false;
    }
    public void CloseJackpotScreen()
    {
        gameplayCamera.SetActive(true);
        gameplayCanvas.SetActive(true);
        MainMenuHandler.instance.mainMenuBg.SetActive(true);

        //if(gameplayBg) gameplayBg.SetActive(true);
        //gameCompleteCanvas.SetActive(true);
        if (jackpotScreen != null)
            Destroy(jackpotScreen);
    }
    public void CloseRewardScreen()
    {
        Debug.LogError("close");
        gameplayCamera.SetActive(true);
        gameplayCanvas.SetActive(true);
        plushieInventoryCanvas.SetActive(true);
        gameplayBg.SetActive(false);
        MainMenuHandler.instance.mainMenuBg.SetActive(true);

        if (jackpotScreen != null)
            Destroy(jackpotScreen);
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IJackpotHandler>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IJackpotHandler>(this);
    }
}
