using System.Collections;
using UnityEngine;

public class EffectsHandler : MonoBehaviour
{
    [SerializeField] GameObject pointSelectionEffectPrefab;
    [SerializeField] Vector3 particleSize;
    [SerializeField] GameObject smallConfettiPrefab;
    [SerializeField] GameObject gameCompleteConfettiPrefab;
    [SerializeField] ParticleSystem[] confettiEffect;
    [SerializeField]int confettiIndex;
    [SerializeField] GameObject[] fireworksEffect;
    [SerializeField] GameObject sparkleTrail;
    [SerializeField] GameObject coinBurstEffect;
    [SerializeField] GameObject sparkleTrailAtTheStitchCompletion;
    
    private void OnEnable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.Register(InstantiateEffect);
        GameEvents.EffectHandlerEvents.onGetInstantiatedEffect.Register(GetGameWinInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onPartCompleteEffect.Register(GetPartCompleteInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onSewnCompletely.Register(StartConfetti);
        GameEvents.EffectHandlerEvents.onSparkleTrailEffect.Register(SparkleTrailEffect);
        GameEvents.EffectHandlerEvents.onSparkleTrailEffectOnCompletion.Register(SparkleTrailEffectOnPlushieComplete);
    }
    private void OnDisable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.UnRegister(InstantiateEffect);
        GameEvents.EffectHandlerEvents.onGetInstantiatedEffect.UnRegister(GetGameWinInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onPartCompleteEffect.UnRegister(GetPartCompleteInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onSewnCompletely.UnRegister(StartConfetti);
        GameEvents.EffectHandlerEvents.onSparkleTrailEffect.UnRegister(SparkleTrailEffect);
        GameEvents.EffectHandlerEvents.onSparkleTrailEffectOnCompletion.UnRegister(SparkleTrailEffectOnPlushieComplete);

    }
    void InstantiateEffect(Transform parent)
    {
        GameObject g = Instantiate(pointSelectionEffectPrefab, parent, false);
        g.transform.SetParent(parent);
        g.transform.localPosition = new Vector3(0, 0, -1f);
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localScale = particleSize;
        g.SetActive(true);
    }

    GameObject GetGameWinInstantiatedEffect(Transform parent)
    {
        GameObject g = Instantiate(gameCompleteConfettiPrefab, parent, false);
        g.transform.SetParent(parent);
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localPosition = Vector3.zero;
        return g;
    }

    GameObject GetPartCompleteInstantiatedEffect(Transform parent)
    {
        GameObject g = Instantiate(gameCompleteConfettiPrefab, parent, false);
        g.transform.SetParent(parent);
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localPosition = Vector3.zero;
        g.transform.SetParent(null);
        return g;
    }
    void StartConfetti()
    {
        StartCoroutine(EnableEffect());
    }
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.fireWorksSound;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
    void DisableAllConfetti()
    {
        foreach (ParticleSystem p in confettiEffect)
        {
            p.gameObject.SetActive(false);
        }
        confettiIndex = 0;
    }
    int fireworksIndex = 0;
    IEnumerator EnableEffect()
    {
        if (confettiIndex < confettiEffect.Length)
        {
            confettiEffect[confettiIndex].gameObject.SetActive(true);
            confettiEffect[confettiIndex].Play();
        }
        if(fireworksIndex < fireworksEffect.Length)
        {
            fireworksEffect[fireworksIndex].SetActive(true);
            fireworksEffect[fireworksIndex].GetComponent<ParticleSystem>().Play();
            PlaySound();
        }
        else
        {
            fireworksIndex = 0;
        }
        yield return new WaitForSeconds(0.15f);
        confettiIndex++;
        fireworksIndex++;
        StopCoroutine(EnableEffect());

        if (confettiIndex <= (confettiEffect.Length - 1))
            StartCoroutine(EnableEffect());
        else
        {
            DisableAllConfetti();
            GameEvents.GameCompleteEvents.onGameComplete.Raise();
        }

    }
    void SparkleTrailEffect(Transform parent)
    {
        GameObject g = Instantiate(sparkleTrail, parent, false);
        g.transform.SetParent(parent);
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localPosition = Vector3.zero;
    }
    GameObject SparkleTrailEffectOnPlushieComplete(Transform parent)
    {
        GameObject g = Instantiate(sparkleTrailAtTheStitchCompletion, parent, false);
        g.transform.SetParent(parent);
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.localPosition = Vector3.zero;
     
        return g;
    }
}
