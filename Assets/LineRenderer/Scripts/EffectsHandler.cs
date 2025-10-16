using UnityEngine;

public class EffectsHandler : MonoBehaviour
{
    [SerializeField] GameObject pointSelectionEffectPrefab;
    [SerializeField] Vector3 particleSize;
    [SerializeField] GameObject smallConfettiPrefab;
    [SerializeField] GameObject gameCompleteConfettiPrefab;
    private void OnEnable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.RegisterEvent(InstantiateEffect);
        GameEvents.EffectHandlerEvents.onGetInstantiatedEffect.RegisterEvent(GetGameWinInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onPartCompleteEffect.RegisterEvent(GetPartCompleteInstantiatedEffect);
    }
    private void OnDisable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.UnregisterEvent(InstantiateEffect);
        GameEvents.EffectHandlerEvents.onGetInstantiatedEffect.UnregisterEvent(GetGameWinInstantiatedEffect);
        GameEvents.EffectHandlerEvents.onPartCompleteEffect.UnregisterEvent(GetPartCompleteInstantiatedEffect);

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
        return g;
    }
}
