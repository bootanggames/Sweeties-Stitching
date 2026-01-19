using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class CoinsHandler : MonoBehaviour,ICoinsHandler
{
    [field: SerializeField] public CoinsEffectData coinsData { get; private set; }
    [field: SerializeField] public List<GameObject> coinsObjList { get; private set; }

    [SerializeField] int totalCoins;
    [SerializeField] int totalCoinsCloneCount;
    [SerializeField] float xPos, yPos;
    [field: SerializeField] public float coinMoveSpeed { get; private set; }
    [SerializeField] Vector3 targetScaleDown;
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
        coinsData.coinsTextBox.text = amount.ToString();
        coinsData.coinsTextBoxGameplayScreen.text = amount.ToString();
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
    }

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
          
        });

        yield return seq.WaitForCompletion();
    }

   public void PlayCoinSound(AudioSource s)
    {
        s.pitch = coinSoundPitchValue;
        s.volume = 1.0f;
        SoundManager.instance.PlaySound(s, SoundManager.instance.audioClips.coinCollection, false, false, 1, false);
        HepticManager.instance.HapticEffect();
        Vector3 target = new Vector3(1.2f, 1.2f, 1.2f);

        Tween bar = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
            .Raise(coinsData.coinBarForGameplayScreen.transform, target, 0.1f, Ease.InOutFlash);

        bar.OnComplete(() =>
        {
            GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform
            .Raise(coinsData.coinBarForGameplayScreen.transform, Vector3.one, 0.1f, Ease.InOutFlash);
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
        coinsData.coinsEarned.text = target.ToString();
        InvokeRepeating(nameof(PlayCoinSoundOnComplete), 0, 0.1f);
    }

    public void PlayCoinSoundOnComplete()
    {
        //audioSource.Stop();
        SoundManager.instance.StopSound(coinsData.audioSource);
        SoundManager.instance.PlaySound(coinsData.audioSource, SoundManager.instance.audioClips.coinCollection, false, false, 1, false);
        HepticManager.instance.HapticEffect();
        Invoke(nameof(StopCoinSoundOnComplete), 3);
    }
    public void StopCoinSoundOnComplete()
    {
        CancelInvoke(nameof(PlayCoinSoundOnComplete));
    }
}
