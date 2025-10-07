using System.Collections.Generic;
using UnityEngine;

public class SewPoint : MonoBehaviour, ISewPoint
{
    public float zVal;
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public bool selected {  get; private set; }

    [field: SerializeField]public List<Transform> stitchEffect_ThreadPoints {  get; private set; }

    public bool IsSelected()
    {
        return selected;
    }

    public void RegisterService()
    {
        ServiceLocator.RegisterService<ISewPoint>(this);
        GameEvents.SewPointEvents.onSelected.RegisterEvent(Selected);
        GameEvents.SewPointEvents.onPointSelectedStatus.RegisterEvent(IsSelected);
    }

    public void Selected(bool val)
    {
        selected = val;
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ISewPoint>(this);
        GameEvents.SewPointEvents.onSelected.UnregisterEvent(Selected);
        GameEvents.SewPointEvents.onPointSelectedStatus.UnregisterEvent(IsSelected);
    }
}
