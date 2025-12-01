using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class CoinsHandler : MonoBehaviour,ICoinsHandler
{
    [field: SerializeField] public GameObject coinBarForGameplayScreen { get; private set; }
    [field: SerializeField] public GameObject coinBar { get; private set; }
    [field: SerializeField] public TextMeshProUGUI coinsTextBox {  get; private set; }
    [field: SerializeField] public GameObject coinSpritePrefab { get; private set; }
    [field: SerializeField] public TextMeshProUGUI coinsTextBoxGameplayScreen { get; private set; }
    [field: SerializeField] public Transform coinsGameplayTarget { get; private set; }

    [SerializeField] int totalCoins;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int totalCoinsCloneCount;
    [field: SerializeField] public List<GameObject> coinsObjList { get; private set; }
    [SerializeField] Transform coinsUiParent;
    [SerializeField] float xPos, yPos;
    [field: SerializeField] public Transform targetPointToMove { get; private set; }
    [field: SerializeField] public float coinMoveSpeed { get; private set; }
    Tween coinMoveTween = null;
    Tween coinScaleTween = null;
    [SerializeField] TextMeshProUGUI coinsEarned;
    int coinsRewarded = 0;
    [SerializeField] Vector3 targetScaleDown;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float coinsIncrementSpeed = 0;
    Tween coinIncrementTween = null;
    [SerializeField] bool testCoinIncrement = false;
    [SerializeField] int amountToTest = 0;
    private void Start()
    {
        totalCoins = PlayerPrefs.GetInt("Coins");
        UpdateCoins(totalCoins);
        //if (totalCoins == 0)
        //    SaveCoins(10);
        //if (testCoinIncrement)
        //    CoinIncrementAnimation(amountToTest);

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
        coinsTextBoxGameplayScreen.text = amount.ToString();
    }

    public void InstantiateCoins(GameObject coinObj, int total, List<GameObject> coinList, Transform parent)
    {
        for (int i = 0; i < total; i++)
        {
            GameObject g = Instantiate(coinObj, parent, false);
            if (!coinList.Contains(g)) coinList.Add(g);
            g.transform.SetParent(parent);
            g.transform.localPosition = new Vector3(0, 0, -1);
            g.transform.localEulerAngles = Vector3.zero;
        }
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
        coinsRewarded = LevelsHandler.instance.currentLevelMeta.levelScriptable.levelReward;
        coinsEarned.text = coinsRewarded.ToString();
    }
    public IEnumerator MoveCoins(List<GameObject> coinList,Transform _target, GameObject coinsBarObj, float moveSpeed, Ease moveEase,float delay)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < coinList.Count; i++)
        {
            GameObject coinObj = coinList[i];

            PlayCoinSound();

            seq.Join(
                GameEvents.DoTweenAnimationHandlerEvents.onMoveToTargetAnimation
                    .Raise(coinObj.transform, _target.position, moveSpeed, moveEase).SetDelay(delay)
            );

            seq.Join(
                GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
                    .Raise(coinObj.transform, targetScaleDown, moveSpeed, Ease.Linear).SetDelay(delay)
            );
        }

        seq.OnComplete(() =>
        {
            SaveCoins(1);

            Vector3 target = new Vector3(1.2f, 1.2f, 1.2f);

            Tween bar = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
                .Raise(coinsBarObj.transform, target, 0.1f, Ease.InOutFlash);

            bar.OnComplete(() =>
            {
                bar.Kill();
                GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
                    .Raise(coinsBarObj.transform, Vector3.one, 0.1f, Ease.InOutFlash);
            });
        });

        yield return seq.WaitForCompletion();
    }

    void PlayCoinSound()
    {
        //audioSource.Stop();
        SoundManager.instance.StopSound(audioSource);
        SoundManager.instance.PlaySound(audioSource, SoundManager.instance.audioClips.coinCollection, false, false, 1, false);
        HepticManager.instance.HapticEffect();
        CancelInvoke(nameof(PlayCoinSound));
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
   
    public void CoinIncrementAnimation(int targetAmount)
    {
        coinIncrementTween = GameEvents.DoTweenAnimationHandlerEvents.onCountIncrement.Raise(targetAmount, coinsIncrementSpeed, coinsTextBox, Ease.InOutBack);
        if(coinIncrementTween != null)
        {
            coinIncrementTween.OnUpdate(() =>
            {

                Invoke(nameof(PlayCoinSound), 0.1f);
            });
        }
       
        coinIncrementTween.OnComplete(() =>
        {
            coinIncrementTween.Kill();
            coinIncrementTween = null;
        });
    }
}
