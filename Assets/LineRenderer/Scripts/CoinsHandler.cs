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
    Tween coinMoveTween = null;
    [SerializeField] TextMeshProUGUI coinsEarned;
    int coinsRewarded = 0;
    [SerializeField] Vector3 targetScaleDown;
    private void Start()
    {
        totalCoins = PlayerPrefs.GetInt("Coins");
        UpdateCoins(totalCoins);
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
        totalCoins = PlayerPrefs.GetInt("Coins");
        int total = totalCoins + amount;
        PlayerPrefs.SetInt("Coins", total);
        UpdateCoins(total);
    }
    public void UpdateCoins(int amount)
    {
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
        coinsRewarded = LevelsHandler.instance.currentLevelMeta.levelReward;
        coinsEarned.text = coinsRewarded.ToString();
    }
    Sequence seq;
    public IEnumerator MoveCoins()
    {
        coinMoveTween.Kill();
        coinMoveTween = null;
        Sequence coinSeq = DOTween.Sequence();

        for (int i = 0; i < coinsObjList.Count; i++)
        {
            yield return new WaitForSeconds(0.015f);
            coinSeq.Join(GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.Raise(coinsObjList[i].transform, targetPointToMove, coinMoveSpeed, Ease.InOutBack));
            coinSeq.Join(GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(coinsObjList[i].transform, targetScaleDown, coinMoveSpeed, Ease.Linear));
            
            coinMoveTween = coinSeq;
            coinMoveTween.OnComplete(() =>
            {
                SaveCoins(1);
            });
        }
  
        StopCoroutine(MoveCoins());
    }

    public void ResetCoinList()
    {
        foreach(GameObject c in coinsObjList)
        {
            Destroy(c);
        }
        coinsObjList.Clear();
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
