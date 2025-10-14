using UnityEngine;

public class EffectsHandler : MonoBehaviour
{
    [SerializeField] GameObject pointSelectionEffectPrefab;

    private void OnEnable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.RegisterEvent(InstantiateEffect);
    }
    private void OnDisable()
    {
        GameEvents.EffectHandlerEvents.onSelectionEffect.UnregisterEvent(InstantiateEffect);
    }
    void InstantiateEffect(Transform parent)
    {
        GameObject g = Instantiate(pointSelectionEffectPrefab, parent, false);
        g.transform.SetParent(parent);
        g.transform.localPosition = new Vector3(0, 0, -1f);
        g.transform.localEulerAngles = Vector3.zero;
        g.SetActive(true);
    }
}
