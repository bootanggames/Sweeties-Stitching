using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class CoinsHandler : MonoBehaviour,ICoinsHandler
{
    [SerializeField] GameObject coinBar;
    [field: SerializeField] public TextMeshProUGUI coinsTextBox {  get; private set; }
    [SerializeField] int totalCoins;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int totalCoinsCloneCount;
    [SerializeField] List<GameObject> coinsObjList;
    [SerializeField] Transform coinsUiParent;
    [SerializeField] float xPos, yPos;
    [SerializeField] Transform targetPointToMove;
    [SerializeField] float coinMoveSpeed;
    Tween coinMoveTween = null;
    Tween coinScaleTween = null;
    [SerializeField] TextMeshProUGUI coinsEarned;
    int coinsRewarded = 0;
    [SerializeField] Vector3 targetScaleDown;
    [SerializeField] AudioSource audioSource;
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
    public IEnumerator MoveCoins()
    {
        for (int i = 0; i < coinsObjList.Count; i++)
        {
            GameObject coinObj = coinsObjList[i];

            yield return new WaitForSeconds(0.015f);
            if (coinObj != null)
            {
                coinMoveTween = GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation.Raise(coinObj.transform, targetPointToMove, coinMoveSpeed, Ease.InOutBack);
                coinScaleTween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(coinObj.transform, targetScaleDown, coinMoveSpeed, Ease.Linear);

                coinMoveTween.OnComplete(() =>
                {
                    SaveCoins(1);
                    PlayCoinSound();
                    Vector3 target = new Vector3(1.2f, 1.2f, 1.2f);
                    Tween bar = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(coinBar.transform, target, 0.1f, Ease.InOutFlash);
                    bar.OnComplete(() =>
                    {
                        bar.Kill();
                        bar = null;
                        bar = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(coinBar.transform, Vector3.one, 0.1f, Ease.InOutFlash);
                    });

                });
            } 
        
        }
  
        StopCoroutine(MoveCoins());
    }

    void PlayCoinSound()
    {
        //audioSource.Stop();
        SoundManager.instance.PlaySound(audioSource, SoundManager.instance.audioClips.coinCollection, false, false, 1, false);
        HepticManager.instance.HapticEffect();
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
