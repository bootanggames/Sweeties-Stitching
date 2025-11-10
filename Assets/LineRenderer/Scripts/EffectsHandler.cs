using System.Collections;
using UnityEngine;

public class EffectsHandler : MonoBehaviour
{
    [SerializeField] GameObject pointSelectionEffectPrefab;
    [SerializeField] Vector3 particleSize;
    [SerializeField] GameObject smallConfettiPrefab;
    [SerializeField] GameObject gameCompleteConfettiPrefab;
    [SerializeField] ParticleSystem[] confettiEffect;
    int confettiIndex;
    private void OnEnable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.RegisterEvent(InstantiateEffect);
        GameEvents.EffectHandlerEvents.onGetInstantiatedEffect.RegisterEvent(GetGameWinInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onPartCompleteEffect.RegisterEvent(GetPartCompleteInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onSewnCompletely.RegisterEvent(StartConfetti);
    }
    private void OnDisable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.UnregisterEvent(InstantiateEffect);
        GameEvents.EffectHandlerEvents.onGetInstantiatedEffect.UnregisterEvent(GetGameWinInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onPartCompleteEffect.UnregisterEvent(GetPartCompleteInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onSewnCompletely.UnregisterEvent(StartConfetti);
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

    void DisableAllConfetti()
    {
        foreach (ParticleSystem p in confettiEffect)
        {
            p.gameObject.SetActive(false);
        }
        confettiIndex = 0;
    }
    IEnumerator EnableEffect()
    {
        if (confettiIndex < confettiEffect.Length)
        {
            confettiEffect[confettiIndex].gameObject.SetActive(true);
            confettiEffect[confettiIndex].Play();
        }
        yield return new WaitForSeconds(0.15f);
        confettiIndex++;
        StopCoroutine(EnableEffect());
        if (confettiIndex <= (confettiEffect.Length - 1))
            StartCoroutine(EnableEffect());
        else
        {
            DisableAllConfetti();
            GameEvents.GameCompleteEvents.onGameComplete.RaiseEvent();

        }
    }
}
