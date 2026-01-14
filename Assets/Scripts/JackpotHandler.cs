using System.Collections.Generic;
using UnityEngine;

public class JackpotHandler : MonoBehaviour, IJackpotHandler
{
    [field: SerializeField] public GameObject jackpotPrefab {  get; private set; }
    [field: SerializeField] public GameObject mainCamera {  get; private set; }
    [field: SerializeField] public GameObject mainMenu {  get; private set; }
    [field: SerializeField] public GameObject mainMenuCanvas {  get; private set; }
    [field: SerializeField] public GameObject roomDecorCanvas {  get; private set; }
    [field: SerializeField] public GameObject plushieInventoryCanvas {  get; private set; }
    [field: SerializeField] public List<ItemsMetaData> earnedItems { get; private set; }
    [field: SerializeField] public GameObject roomBg { get; private set; }

    GameObject jackpotScreen = null;
    IRoomdecorStore _roomdecorScreen;
    private void OnEnable()
    {
        RegisterService();
    }
    private void Start()
    {
        _roomdecorScreen = ServiceLocator.GetService<IRoomdecorStore>();
        earnedItems = new List<ItemsMetaData>();
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
        mainCamera.SetActive(false);
        mainMenuCanvas.SetActive(false);
        roomDecorCanvas.SetActive(false);
        plushieInventoryCanvas.SetActive(false);
        roomBg.SetActive(false);
        CancelInvoke(nameof(ShowWithDelay));
    }
    public void CloseJackpotScreen()
    {
        mainCamera.SetActive(true);
        mainMenuCanvas.SetActive(true);
        roomBg.SetActive(true);
        if (jackpotScreen != null)
            Destroy(jackpotScreen);
    }
    public void CloseRewardScreen()
    {
        mainCamera.SetActive(true);
        mainMenuCanvas.SetActive(true);
        //plushieInventoryCanvas.SetActive(true);//----
        _roomdecorScreen.MyRoomButton();
        mainMenu.SetActive(false);
        roomBg.SetActive(true);

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
