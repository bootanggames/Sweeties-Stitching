using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SewPoint : MonoBehaviour, ISewPoint
{
    public float zVal;
    public int attachmentId;
    public bool connected = false;
    public SequenceType sequenceType;
    [SerializeField] TextMeshPro textObj;
    [HideInInspector]public MeshRenderer pointMesh;
    public bool selected {  get; private set; }
    [field: SerializeField]public List<Transform> stitchEffect_ThreadPoints {  get; private set; }
    private void OnEnable()
    {
        pointMesh = GetComponent<MeshRenderer>();
        textObj = GetComponentInChildren<TextMeshPro>();
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
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
    public void ChangeText(string s)
    {
        textObj.text = s;
    }
    public void ChangeTextColor(Color c)
    {
        if(textObj)
            textObj.color = c;
    }
}
