using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface ICoinsHandler : IGameService
{
    GameObject coinBar {  get; }
    GameObject coinBarForGameplayScreen {  get; }
    Transform targetPointToMove {  get; }
    List<GameObject> coinsObjList {  get; }
    TextMeshProUGUI coinsTextBox {  get; }
    TextMeshProUGUI coinsTextBoxGameplayScreen {  get; }
    GameObject coinSpritePrefab {  get; }
    Transform coinsGameplayTarget {  get; }
    float coinMoveSpeed {  get; }
    void SaveCoins(int amount);
    int GetCoins();
    void UpdateCoins(int amount);
    void CreateCoinsObjects();
    IEnumerator MoveCoins(List<GameObject> coinList, Transform target, GameObject coinsBarObj, float moveSpeed, Ease moveEase, float delay);
    void ResetCoinList();
    void InstantiateCoins(GameObject coinObj, int total, List<GameObject> coinList, Transform parent);
    void CoinIncrementAnimation(int targetAmount);
}
