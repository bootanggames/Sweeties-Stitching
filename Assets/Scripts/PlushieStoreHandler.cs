using TMPro;
using UnityEngine;

public class PlushieStoreHandler : MonoBehaviour, IPlushieStoreHandler
{
    [field: SerializeField] public int totalPlushies {  get; private set; }
    [field: SerializeField] public int plushiesInUserInventory { get; private set; }
    [SerializeField] TextMeshProUGUI availablePlushieCount;
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IPlushieStoreHandler>(this);
    }
    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPlushieStoreHandler>(this);
    }
    public void GetPlushieCountUI()
    {
        availablePlushieCount.text = plushiesInUserInventory.ToString();
    }
}
