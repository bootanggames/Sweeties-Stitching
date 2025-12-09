using UnityEngine;

public interface IPlushieStoreHandler : IGameService
{
    int totalPlushies { get; }
    int plushiesInUserInventory {  get; }
    void GetPlushieCountUI();
}
