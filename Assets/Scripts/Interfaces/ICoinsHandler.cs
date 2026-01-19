using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface ICoinsHandler : IGameService
{
    List<GameObject> coinsObjList {  get; }
    CoinsEffectData coinsData { get; }
    float coinMoveSpeed {  get; }
    //AudioSource audioSource {  get; }
    void SaveCoins(int amount);
    int GetCoins();
    void UpdateCoins(int amount);
    //void CreateCoinsObjects();
    IEnumerator MoveCoins(List<GameObject> coinList, Transform target, GameObject coinsBarObj, float moveSpeed, Ease moveEase, float delay, bool randomSpeed);
    void ResetCoinList();
    void InstantiateCoins(GameObject coinObj, int total, List<GameObject> coinList, Transform parent);
    void CoinIncrementAnimation(int targetAmount);
    void StopCoinSound();
    void PlayCoinSoundOnComplete();
    void InstantiateSingleCoin(GameObject coinObj, List<GameObject> coinList, Transform parent);
    void PlayCoinSound(AudioSource s);
    void StopCoinSoundOnComplete();
}
