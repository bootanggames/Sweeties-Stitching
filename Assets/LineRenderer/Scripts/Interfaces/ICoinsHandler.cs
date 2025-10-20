using System.Collections;
using UnityEngine;

public interface ICoinsHandler : IGameService
{
    void SaveCoins(int amount);
    int GetCoins();
    void UpdateCoins(int amount);
    void CreateCoinsObjects();
    IEnumerator MoveCoins();
}
