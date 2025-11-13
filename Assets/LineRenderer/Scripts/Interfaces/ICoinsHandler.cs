using System.Collections;
using TMPro;
using UnityEngine;

public interface ICoinsHandler : IGameService
{
    TextMeshProUGUI coinsTextBox {  get; }
    void SaveCoins(int amount);
    int GetCoins();
    void UpdateCoins(int amount);
    void CreateCoinsObjects();
    IEnumerator MoveCoins();
    void ResetCoinList();
}
