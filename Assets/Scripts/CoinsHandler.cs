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
    [field:SerializeField] public AudioSource audioSource {  get; private set; }
    [SerializeField] float coinsIncrementSpeed = 0;
    Tween coinIncrementTween = null;
    [SerializeField] bool testCoinIncrement = false;
    [SerializeField] int amountToTest = 0;
    [SerializeField]float timeDuration = 0;
    [SerializeField] float waitTime;
    [SerializeField] float coinSoundPitchValue;
    private void Start()
    {
        timeDuration = Time.time;

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
        //AudioSource s = coinObj.GetComponent<AudioSource>();
        //PlayCoinSound(s);
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
            InstantiateSingleCoin(coinObj, coinList, parent);

        }
    }

    public void InstantiateSingleCoin(GameObject coinObj,List<GameObject> coinList, Transform parent)
    {
        GameObject g = Instantiate(coinObj, parent, false);
        if (!coinList.Contains(g)) coinList.Add(g);
        g.transform.SetParent(parent);
        g.transform.localPosition = new Vector3(0, 0, -1);
        g.transform.localEulerAngles = Vector3.zero;
        //coinList.AddRange(coinsObjList);
        //Debug.LogError(" " + coinList.Count);
    }
    //public void CreateCoinsObjects()
    //{
    //    for (int i = 0; i < totalCoinsCloneCount; i++)
    //    {
    //        GameObject g = Instantiate(coinPrefab, coinsUiParent, false);
    //        coinsObjList.Add(g);
    //        g.transform.SetParent(coinsUiParent);
    //        float x = Random.Range(-xPos, xPos);
    //        float y = Random.Range(-yPos, yPos);
    //        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    //    }
    //    coinsRewarded = LevelsHandler.instance.currentLevelMeta.levelScriptable.levelReward;
    //    coinsEarned.text = coinsRewarded.ToString();
    //}
    public IEnumerator MoveCoins(List<GameObject> coinList,Transform _target, GameObject coinsBarObj, float moveSpeed, Ease moveEase,float delay,bool randomSpeed)
    {
        Sequence seq = DOTween.Sequence();
        GameObject coinObj = null;
        for (int i = 0; i < coinList.Count; i++)
        {
            if (randomSpeed)
            {
                float speed = Random.Range(0.01f, 1.0f);
                moveSpeed = speed;
            }

            coinObj = coinList[i];
            //coinObj.AddComponent<AudioSource>();
            //AudioSource s = coinObj.GetComponent<AudioSource>();
            //PlayCoinSound(s);
            //Debug.LogError("coin movement" + coinList.Count);
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
            //SaveCoins(1);
            //if (coinObj != null)
            //{
            //    coinObj.AddComponent<AudioSource>();
            //    AudioSource s = coinObj.GetComponent<AudioSource>();
            //    PlayCoinSound(s);
            //}
            //Debug.LogError("Seq complete");
            //Vector3 target = new Vector3(1.2f, 1.2f, 1.2f);

            //Tween bar = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
            //    .Raise(coinsBarObj.transform, target, 0.1f, Ease.InOutFlash);

            //bar.OnComplete(() =>
            //{
            //    bar.Kill();
            //    StopCoroutine(MoveCoins(coinList, _target, coinsBarObj, moveSpeed, moveEase, delay, randomSpeed));
            //    GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
            //        .Raise(coinsBarObj.transform, Vector3.one, 0.1f, Ease.InOutFlash);
            //});
        });

        yield return seq.WaitForCompletion();
    }

   public void PlayCoinSound(AudioSource s)
    {
        //audioSource.Stop();
        //SoundManager.instance.StopSound(s);
        s.pitch = coinSoundPitchValue;
        s.volume = 1.0f;
        SoundManager.instance.PlaySound(s, SoundManager.instance.audioClips.coinCollection, false, false, 1, false);
        HepticManager.instance.HapticEffect();
        Vector3 target = new Vector3(1.2f, 1.2f, 1.2f);

        Tween bar = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
            .Raise(coinBarForGameplayScreen.transform, target, 0.1f, Ease.InOutFlash);

        bar.OnComplete(() =>
        {
            GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
            .Raise(coinBarForGameplayScreen.transform, Vector3.one, 0.1f, Ease.InOutFlash);
            bar.Kill();

        });
        Invoke(nameof(StopCoinSound), 1);
    }
    public void StopCoinSound()
    {
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
        int actualAmount = PlayerPrefs.GetInt("Coins");
        int target = actualAmount + targetAmount;
        PlayerPrefs.SetInt("Coins", target);
        coinIncrementTween = GameEvents.DoTweenAnimationHandlerEvents.onCountIncrement.Raise(target, coinsIncrementSpeed, Ease.InOutBack);
        if (coinIncrementTween != null)
        {
            coinIncrementTween.OnUpdate(() =>
            {
                //Invoke(nameof(PlayCoinSound), 0.1f);
            });
        }

        coinIncrementTween.OnComplete(() =>
        {
            coinIncrementTween.Kill();
            coinIncrementTween = null;
        });
        coinsEarned.text = target.ToString();
        InvokeRepeating(nameof(PlayCoinSoundOnComplete), 0, 0.1f);
    }

    public void PlayCoinSoundOnComplete()
    {
        //audioSource.Stop();
        SoundManager.instance.StopSound(audioSource);
        SoundManager.instance.PlaySound(audioSource, SoundManager.instance.audioClips.coinCollection, false, false, 1, false);
        HepticManager.instance.HapticEffect();
        Invoke(nameof(StopCoinSoundOnComplete), 3);
    }
    public void StopCoinSoundOnComplete()
    {
        CancelInvoke(nameof(PlayCoinSoundOnComplete));
    }
}
