using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SewPoint : MonoBehaviour, ISewPoint
{
    public float zVal;
    public int attachmentId;
    public int nextConnectedPointId;
    public SewPointMetaData metaData;
    public SequenceType sequenceType;
    [HideInInspector] TextMeshPro textObj;
    [HideInInspector]public MeshRenderer pointMesh;
    public bool selected {  get; private set; }
    //[field: SerializeField] public List<Transform> stitchEffect_ThreadPoints {  get; private set; }
    [field: SerializeField] public bool startFlag { get; private set; }
    [HideInInspector]public Vector3 originalScale;

    public Transform cleanStitchPoint;
    ObjectInfo parentInfo;
    private void OnEnable()
    {
        parentInfo = this.gameObject.GetComponentInParent<ObjectInfo>();
        originalScale = this.transform.localScale;
        pointMesh = GetComponent<MeshRenderer>();
        textObj = GetComponentInChildren<TextMeshPro>();
        RegisterService();

        Invoke(nameof(LoadSavedData), 0.21f);
    }

    public void LoadSavedData()
    {
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler != null)
        {
            if (gameHandler.saveProgress)
            {
                int plushieIndex = PlayerPrefs.GetInt("Level_" + LevelsHandler.instance.levelIndex + "_Plushie");
                string bodyPart = parentInfo.partType.ToString();
                if (SaveDataUsingJson.instance)
                    metaData = SaveDataUsingJson.instance.LoadData<SewPointMetaData>(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + plushieIndex + "_" + bodyPart + "_" + attachmentId);

                if (metaData == null)
                    metaData = new SewPointMetaData();
                if (metaData.connected)
                {
                    var threadHandler = ServiceLocator.GetService<IThreadManager>();
                    var pointsHandler = ServiceLocator.GetService<IPointConnectionHandler>();
                    var needleDetector = ServiceLocator.GetService<INeedleDetector>();

                    if (!parentInfo.stitchData.IsStitched)
                    {
                        if (threadHandler != null)
                        {
                            if (!threadHandler.detectedPoints.Contains(this.transform))
                                threadHandler.detectedPoints.Add(this.transform);

                        }
                        if (pointsHandler != null)
                        {
                            if (!pointsHandler.points.Contains(this))
                                pointsHandler.points.Add(this);

                            pointMesh.material = pointsHandler.correctPointMaterial;
                        }
                        if (needleDetector != null)
                        {
                            if (!needleDetector.pointsDetected.Contains(this))
                                needleDetector.pointsDetected.Add(this);

                        }
                    }
                    else
                    {
                        if (threadHandler != null)
                            threadHandler.detectedPoints.Clear();
                        if (pointsHandler != null)
                            pointsHandler.points.Clear();
                        if (needleDetector != null)
                            needleDetector.pointsDetected.Clear();
                    }
                }
            }
            else
            {
                int plushieIndex = PlayerPrefs.GetInt("Level_" + LevelsHandler.instance.levelIndex + "_Plushie");
                string bodyPart = parentInfo.partType.ToString();

                if (SaveDataUsingJson.instance)
                    SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + plushieIndex + "_" + bodyPart + "_" + attachmentId, metaData);
            }
        }
        CancelInvoke(nameof(LoadSavedData));
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
  
    public void IsConnected(bool val, int v, Vector3 lastMovemPosition,string bodyPart)
    {
        metaData.connected = val;
        int plushieIndex = PlayerPrefs.GetInt("Level_" + LevelsHandler.instance.levelIndex + "_Plushie");

        if (SaveDataUsingJson.instance)
            SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + plushieIndex + "_" + bodyPart + "_" + attachmentId, metaData);
    }
}
