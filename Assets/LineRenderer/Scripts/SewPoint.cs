using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SewPoint : MonoBehaviour, ISewPoint
{
    public float zVal;
    public int attachmentId;
    public int nextConnectedPointId;
    public bool connected = false;
    public SequenceType sequenceType;
    [HideInInspector] TextMeshPro textObj;
    [HideInInspector]public MeshRenderer pointMesh;
    public bool selected {  get; private set; }
    [field: SerializeField] public List<Transform> stitchEffect_ThreadPoints {  get; private set; }
    [field: SerializeField] public bool startFlag { get; private set; }
    [HideInInspector]public Vector3 originalScale;

    public Transform cleanStitchPoint;
    private void OnEnable()
    {
        originalScale = this.transform.localScale;

        pointMesh = GetComponent<MeshRenderer>();
        textObj = GetComponentInChildren<TextMeshPro>();
        RegisterService();

        //var gameHandler = ServiceLocator.GetService<IGameHandler>();
        //if (gameHandler != null)
        //{
        //    if (gameHandler.saveProgress)
        //    {
        //        int stitched = PlayerPrefs.GetInt(attachmentId.ToString() + "_IsConnected");
        //        if (stitched == 1)
        //        {
        //            connected = true;
        //            //parentInfo.noOfConnections++;
        //        }
        //    }
        //    else
        //    {
        //        PlayerPrefs.SetInt(attachmentId.ToString() + "_IsConnected", 0);
        //    }
        //}
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

    public void IsConnected(bool val, int v)
    {
        connected = val;
        PlayerPrefs.SetInt(attachmentId.ToString() + "_IsConnected", v);
    }
}
