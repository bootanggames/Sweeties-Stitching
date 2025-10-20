using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsHandler : MonoBehaviour,ICoinsHandler
{
    [SerializeField] TextMeshProUGUI coinsTextBox;
    [SerializeField] int totalCoins;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int totalCoinsCloneCount;
    [SerializeField] List<GameObject> coinsObjList;
    [SerializeField] Transform coinsUiParent;
    [SerializeField] float xPos, yPos;
    [SerializeField] Transform targetPointToMove;
    [SerializeField] float coinMoveSpeed;
    private void Start()
    {
        if (totalCoins == 0)
            SaveCoins(10);
    }
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public int GetCoins()
    {
        return totalCoins;
    }
    public void SaveCoins(int amount)
    {
        PlayerPrefs.SetInt("Coins", amount);
        UpdateCoins(amount);
    }
    public void UpdateCoins(int amount)
    {
        totalCoins = PlayerPrefs.GetInt("Coins");
        coinsTextBox.text = amount.ToString();
    }
    public void CreateCoinsObjects()
    {
        for (int i = 0; i < totalCoinsCloneCount; i++)
        {
            GameObject g = Instantiate(coinPrefab, coinsUiParent, false);
            coinsObjList.Add(g);
            g.transform.SetParent(coinsUiParent);
            float x = Random.Range(-xPos, xPos);
            float y = Random.Range(-yPos, yPos);
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }

    }
    public IEnumerator MoveCoins()
    {

        for (int i = 0; i < coinsObjList.Count; i++)
        {
            yield return new WaitForSeconds(0.015f);
            GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.RaiseEvent(coinsObjList[i].transform, targetPointToMove, coinMoveSpeed, Ease.InOutBack);
        }
        StopCoroutine(MoveCoins());
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<ICoinsHandler>(this);
    }
    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ICoinsHandler>(this);
    }

   
}
