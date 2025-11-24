using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;

public class Level_Metadata : MonoBehaviour
{
    //public LevelDataScriptable levelScriptable;
    public string levelName;
    public int levelReward = 0;
    public int totalCorrectLinks;
    public int noOfLinks;
    public PlushieActiveStitchPart plushieActivePartToStitch;
    public List<GameObject> bodyParts;
    public Part_Info head;
    public GameObject immoveablePart;
    public GameObject bodyWihtoutHoles;
    public GameObject sewnPlushie;

    public int totalStitchedPart;
    public int noOfStitchedPart;
    ObjectInfo current_ObjectInfor = null;

    [SerializeField] LevelDivision levelDivision;
    [SerializeField]SequenceType sequenceType;

    [SerializeField] Transform neckCamera;
    [SerializeField] Transform leftEyeCamera;
    [SerializeField] Transform leftEarCamera;
    [SerializeField] Transform rightEarCamera;
    [SerializeField] Transform rightEyeCamera;
    [SerializeField] Transform rightArmCamera;
    [SerializeField] Transform rightLegCamera;
    [SerializeField] Transform leftLegCamera;
    [SerializeField] Transform leftArmCamera;
    [SerializeField] Transform gameCompleteCamera;
    [SerializeField] Transform gameHalfProgressCamera;
    [Header("----------GameCompleteScreen---------")]
    public float plushieWidth;
    public float plushieHeight;
    public Sprite plushieSprite;
    [SerializeField] Transform needleUndoPosition;
    public Color threadColor;
    private void Start()
    {
        LevelInitialisation();
    }
   public void LevelInitialisation()
    {
        HandlerPointsEnableDisable();
        RepositionCameras();
        UpdateAllStitchesOfPlushie();
    }
    void RepositionCameras()
    {
        var cameraHandler = ServiceLocator.GetService<ICameraManager>();
        if(cameraHandler != null )
        {
            cameraHandler.RepositionCamera(cameraHandler.neckCamera.transform, neckCamera);
            cameraHandler.RepositionCamera(cameraHandler.leftEyeCamera.transform, leftEyeCamera);
            cameraHandler.RepositionCamera(cameraHandler.leftEarCamera.transform, leftEarCamera);
            cameraHandler.RepositionCamera(cameraHandler.rightEarCamera.transform, rightEarCamera);
            cameraHandler.RepositionCamera(cameraHandler.rightEyeCamera.transform, rightEyeCamera);
            cameraHandler.RepositionCamera(cameraHandler.rightArmCamera.transform, rightArmCamera);
            cameraHandler.RepositionCamera(cameraHandler.rightLegCamera.transform, rightLegCamera);
            cameraHandler.RepositionCamera(cameraHandler.leftLegCamera.transform, leftLegCamera);
            cameraHandler.RepositionCamera(cameraHandler.leftArmCamera.transform, leftArmCamera);
        }
    }
    void EnableDisableSewPoints(List<SewPoint> points, bool val)
    {
        foreach (SewPoint s in points)
        {
            //s.GetComponent<Collider>().enabled = val;
            if(s!=null)
                s.gameObject.SetActive(val);
        }
    }
    void GetAllPartsObjects(List<GameObject> parts)
    {
        foreach (GameObject g in parts)
        {
            ObjectInfo o = g.GetComponent<ObjectInfo>();
            if(o!=null)
                EnableDisableSewPoints(o.connectPoints, false);
        }
    }
    void HandlerPointsEnableDisable()
    {
        GetAllPartsObjects(bodyParts);
        GetAllPartsObjects(head.joints);
        if(immoveablePart.GetComponent<Part_Info>())
            GetAllPartsObjects(immoveablePart.GetComponent<Part_Info>().joints);
    }
    public ObjectInfo GetObjectInfoOfCurrentUnstitchedPart(List<GameObject> list)
    {
        ObjectInfo ob_info = null;
        foreach (GameObject g in list)
        {
            ObjectInfo o = g.GetComponent<ObjectInfo>();
            if (o.stitchData == null)
                o.stitchData = new PlushiePartStitchData();
            if (!o.stitchData.IsStitched)
            {
                ob_info = o;
                break;
            }


        }
        return ob_info;
    }
    public void StartLevel() 
    {

        ObjectInfo currentConnectedPartInfor = null;
        ObjectInfo neck = bodyParts[0].GetComponent<ObjectInfo>();
        if (neck.stitchData.IsStitched)
            current_ObjectInfor = GetObjectInfoOfCurrentUnstitchedPart(levelDivision.rightSide);
        else
            current_ObjectInfor = neck;
        if (current_ObjectInfor != null)
        {
            levelDivision.rightSideIndex = levelDivision.rightSide.IndexOf(current_ObjectInfor.gameObject) + 1;
            if (current_ObjectInfor.partConnectedTo.Equals(PartConnectedTo.head))
                currentConnectedPartInfor = GetObjectInfoOfCurrentUnstitchedPart(head.joints);
            else
                currentConnectedPartInfor = GetObjectInfoOfCurrentUnstitchedPart(immoveablePart.GetComponent<Part_Info>().joints);
            NextPartActivation(true, SequenceType.none, currentConnectedPartInfor);
        }
        else
            NextPartActivation(true, SequenceType.none, null);

    }

    void NextPartActivation(bool start, SequenceType sequence, ObjectInfo connectedTo)
    {
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
            needleDetecto.detect = false;

        Part_Info p2_Info = immoveablePart.GetComponent<Part_Info>();
        sequenceType = SequenceType.right;

        if (!start)
        {
            if (levelDivision.rightSideIndex < levelDivision.rightSide.Count)
                current_ObjectInfor = levelDivision.rightSide[levelDivision.rightSideIndex].GetComponent<ObjectInfo>();

            levelDivision.rightSideIndex++;
        }
        if(current_ObjectInfor == null)
            current_ObjectInfor = bodyParts[0].GetComponent<ObjectInfo>();

        EnableDisableSewPoints(current_ObjectInfor.connectPoints, true);
        if (current_ObjectInfor.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(current_ObjectInfor.partType, true);

        if (current_ObjectInfor.partConnectedTo.Equals(PartConnectedTo.head))
            head.EnableJoint(current_ObjectInfor.partType, true);

        CameraFocus(current_ObjectInfor.partType);
        Invoke("EnableDetection", 0.22f);
        //partIndex++;
    }

    IEnumerator CheckProgress(ObjectInfo o_info)
    {
        var cameraManager = ServiceLocator.GetService<ICameraManager>();
        if (cameraManager != null)
            GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.gameHalfProgressCamera);
        yield return new WaitForSeconds(2);
        CameraFocus(o_info.partType);
        StopCoroutine(CheckProgress(o_info));
    }
    void EnableDetection()
    {
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
        {
            needleDetecto.detect = true;
        }
        var threadHandler = ServiceLocator.GetService<IThreadManager>();
        if (threadHandler != null)
        {
            threadHandler.ResetList(threadHandler.detectedPoints.OrderBy(t => t.GetComponent<SewPoint>().attachmentId).ToList());
        }
        var pointsHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if (pointsHandler != null)
        {
            if (pointsHandler.points.Count > 0)
                pointsHandler.ResetPointsList(pointsHandler.points.OrderBy(p => p.attachmentId).ToList());

            for (int i = 0; i < pointsHandler.points.Count; i++)
            {
                if((i+1) < pointsHandler.points.Count)
                {
                    Connections newConnection = new Connections(pointsHandler.points[i].transform, pointsHandler.points[i + 1].transform, pointsHandler.linePrefab, -0.01f, false, 0);
                    pointsHandler.connections.Add(newConnection);
                    if (pointsHandler.points[i].attachmentId.Equals(pointsHandler.points[i + 1].attachmentId))
                        LevelsHandler.instance.currentLevelMeta.Connection(pointsHandler.points[i], pointsHandler.points[i + 1]);
                }
            }

        }
        var needleDetector = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetector != null)
        {
            if (needleDetector.pointsDetected.Count > 0)
                needleDetector.ResetDetectedPointsList(needleDetector.pointsDetected.OrderBy(p => p.attachmentId).ToList());
        }
        CancelInvoke("EnableDetection");
    }
 
    public void UpdateLevelProgress(SequenceType sequence)
    {
        var canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasManager != null)
            canvasManager.UpdatePlushieStitchProgress(totalStitchedPart, noOfStitchedPart);
        var IthreadHandler = ServiceLocator.GetService<IThreadManager>();
        
        if (noOfStitchedPart.Equals(totalStitchedPart))
        {
            Time.timeScale = 1.2f;
            var cameraManager = ServiceLocator.GetService<ICameraManager>();
            if (cameraManager != null)
                GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.gameCompleteCamera);

            if (bodyWihtoutHoles)
            {
                bodyWihtoutHoles.SetActive(true);
                immoveablePart.GetComponent<SpriteRenderer>().enabled = false;
            }
            
            GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
           if(IthreadHandler != null)
                IthreadHandler.SetUndoValue(false);
            PlaySewnSound();
            Invoke("WinEffect", 2.0f);
        }
        else
        {
            if (IthreadHandler != null)
                IthreadHandler.SetUndoValue(true);
            NextPartActivation(false, sequence, null);
        }
    }
    void WinEffect()
    {
        Time.timeScale = 1.0f;

        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if(canvasHandler != null)
            canvasHandler.sewnScreen.SetActive(true);
    }
    void PlaySewnSound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
        HepticManager.instance.HapticEffect();
        //Debug.LogError("completed");
    }
    void CallGameWinPanel()
    {
        GameEvents.GameCompleteEvents.onGameComplete.RaiseEvent();
        CancelInvoke("CallGameWinPanel");
    }
  
    public void UpdateAllStitchesOfPlushie()
    {
        var canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasManager != null)
            canvasManager.UpdateStitchCount(totalCorrectLinks, noOfLinks);
    }
    public void CameraFocus(PlushieActiveStitchPart currentActivePart)
    {
        var cameraManager = ServiceLocator.GetService<ICameraManager>();
        INeedleMovement needleHandler = ServiceLocator.GetService<INeedleMovement>();

        if (cameraManager != null)
        {
            switch (currentActivePart)
            {
                case PlushieActiveStitchPart.neck:
                    plushieActivePartToStitch = PlushieActiveStitchPart.neck;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.neckCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.righteye:
                    plushieActivePartToStitch = PlushieActiveStitchPart.righteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightEyeCamera);
                    if(needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.32f);
                        needleHandler.ChangeMinThreshHoldValue(1.30e-05f);

                    }
                    break;
                case PlushieActiveStitchPart.lefteye:
                    plushieActivePartToStitch = PlushieActiveStitchPart.lefteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftEyeCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.32f);
                        needleHandler.ChangeMinThreshHoldValue(1.30e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightear:
                    plushieActivePartToStitch = PlushieActiveStitchPart.rightear;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightEarCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftear:
                    plushieActivePartToStitch = PlushieActiveStitchPart.leftear;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftEarCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightarm:
                    plushieActivePartToStitch = PlushieActiveStitchPart.rightarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightArmCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftarm:
                    plushieActivePartToStitch = PlushieActiveStitchPart.leftarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftArmCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightleg:
                    plushieActivePartToStitch = PlushieActiveStitchPart.rightleg;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightLegCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftleg:
                    plushieActivePartToStitch = PlushieActiveStitchPart.leftleg;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftLegCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
            }
        }
       
    }
    public void UpdateLinks()
    {
        var sewPointHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if( sewPointHandler != null)
        {
            foreach (GameObject g in bodyParts)
            {
                ObjectInfo ob_info = g.GetComponent<ObjectInfo>();
                if (!ob_info.stitchData.IsStitched)
                {
                    if (ob_info.Equals(current_ObjectInfor))
                    {
                        noOfLinks -= sewPointHandler.connections.Count;
                        break;
                    }
                }
            }
        }
    }
   
    void ResetEveryPart(List<GameObject> parts)
    {
        foreach (GameObject g in parts)
        {
            ObjectInfo o_Info = g.GetComponent<ObjectInfo>();
            if (o_Info.head)
                head.transform.position = o_Info.startPosition;

            o_Info.ResetPart();
        }
    }
    public void ResetLevel()
    {
        ResetEveryPart(bodyParts);
        ResetEveryPart(head.joints);
        ResetEveryPart(immoveablePart.GetComponent<Part_Info>().joints);
        noOfStitchedPart = 0;
        noOfLinks = 0;
    }

    public void Delay()
    {
        Invoke("LoadPRogress", 0.5f);
    }
    public void LoadPRogress()
    {
        foreach (GameObject g in bodyParts)
        {
            ObjectInfo o_Info = g.GetComponent<ObjectInfo>();
            int stitched = PlayerPrefs.GetInt(o_Info.partType.ToString() + "_IsStiched");
            if (stitched == 1)
            {
                if (o_Info.head)
                    o_Info.PartPositioning(head.gameObject, o_Info.movedPosition);
                else
                {
                    if (o_Info.partWithOutHoles == null)
                        o_Info.PartPositioning(g.gameObject, o_Info.movedPosition);
                    o_Info.ChangePartsState(false);
                }
            }
        }

    }
    public void CheckIfStitchedBeforeCompleteScreen()
    {
        if (noOfStitchedPart.Equals(totalStitchedPart))
        {
            if (bodyWihtoutHoles)
            {
                bodyWihtoutHoles.SetActive(false);
                immoveablePart.SetActive(true);
                immoveablePart.GetComponent<SpriteRenderer>().enabled = true;
            }
            GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
            LevelsHandler.instance.LevelIncrementProcess();
           
        }
    }

    public void ResetNeedlePosition(PlushieActiveStitchPart currentActivePart)
    {
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(null, needleUndoPosition.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
        ResetNeedleAndThread(worldPos);
        
    }

    void ResetNeedleAndThread(Vector3 position)
    {
        var threadHandler = ServiceLocator.GetService<IThreadManager>();
        Vector3 threadPos = Vector3.zero;

        if (threadHandler != null && threadHandler.instantiatedLine != null)
        {
            position.z = threadHandler.zVal;
            GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(position);
            threadPos = position;
            threadPos.z = threadHandler.zVal;
            threadHandler.instantiatedLine.SetPosition(0, threadPos);
        }
       
    }
    [SerializeField] LineRenderer linePrefabForCleanConnection;
    LineRenderer lineForCleanConnection;
    public List<Connections> cleanConnection;
    List<Connections> cleanThreads = new List<Connections>();
    public int cleanThreadIndex = 0;
    public void Connection(SewPoint sp1, SewPoint sp2)
    {
        Vector3 pos1 = Vector3.zero; Vector3 pos2 = Vector3.zero;
        if (sp1.cleanStitchPoint != null && sp2.cleanStitchPoint != null)
        {
            this.lineForCleanConnection = GameObject.Instantiate(linePrefabForCleanConnection);

            pos1 = sp1.cleanStitchPoint.position;
            pos2 = sp2.cleanStitchPoint.position;
            this.lineForCleanConnection.positionCount = 2;
            this.lineForCleanConnection.SetPosition(0, pos1);
            this.lineForCleanConnection.SetPosition(1, pos2);
            this.lineForCleanConnection.material.color = threadColor;
            Connections connection = new Connections(sp1.cleanStitchPoint, sp2.cleanStitchPoint, linePrefabForCleanConnection, -0.01f, false, 2);
            cleanConnection.Add(connection);
            connection.line.gameObject.SetActive(false);
            connection.isLocked = true;
            cleanThreads.Add(connection);
        }
       
    }
    public void UpdateCleanThreadConnections()
    {
        if (cleanConnection.Count > 0)
        {
            foreach(Connections c in cleanConnection)
            {
                c.UpdateLine(-0.01f, false);
            }
        }
    }
    public void DeactivateAllThreads()
    {
        foreach(var connections in cleanThreads)
        {
            connections.line.gameObject.SetActive(false);
        }
    }
}
