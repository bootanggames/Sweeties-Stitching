using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Level_Metadata : MonoBehaviour
{
    public bool completed = false;
    public LevelDataScriptable levelScriptable;
    public int noOfStitchesDone;
    public int noOfStitchedPart;
    public PlushieActiveStitchPart currentActivePart;
    public List<GameObject> bodyParts;
    public Part_Info head;
    public GameObject immoveablePart;
    public GameObject bodyWihtoutHoles;
    public GameObject sewnPlushie;
    [HideInInspector]public ObjectInfo current_ObjectInfor = null;
    [SerializeField] ObjectInfo stitchStartingPart;
    [SerializeField] Transform needleUndoPosition;
    [SerializeField] LineRenderer linePrefabForCleanConnection;

    LineRenderer lineForCleanConnection;
    [HideInInspector] public List<Connections> cleanConnection;
    [HideInInspector] public int cleanThreadIndex = 0;
    [HideInInspector] public List<GameObject> crissCrossObjList = new List<GameObject>();
    List<Connections> cleanThreads = new List<Connections>();

    //[SerializeField] Transform neckCamera;
    //[SerializeField] Transform leftEyeCamera;
    //[SerializeField] Transform leftEarCamera;
    //[SerializeField] Transform rightEarCamera;
    //[SerializeField] Transform rightEyeCamera;
    //[SerializeField] Transform rightArmCamera;
    //[SerializeField] Transform rightLegCamera;
    //[SerializeField] Transform leftLegCamera;
    //[SerializeField] Transform leftArmCamera;
    //[SerializeField] Transform gameCompleteCamera;
    //[SerializeField] Transform gameHalfProgressCamera;
    //public string levelName;
    //public int levelReward = 0;
    //public int totalCorrectLinks;
    //public int totalStitchedPart;
    //[SerializeField] LevelDivision levelDivision;
    //[SerializeField]SequenceType sequenceType;
    //public float plushieWidth;
    //public float plushieHeight;
    //public Sprite plushieSprite;
    //public Color threadColor;
    //public Sprite spoolColor;
    //public GameObject stitchObj;
    //public GameObject crissCrossObjForEyes;

    private void Start()
    {
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
            canvasHandler.spoolImg.sprite = levelScriptable.threadSpool;
        LevelInitialisation();
    }
    public void LevelInitialisation()
    {
        DisableAllSewPoints();
        RepositionCameras();
        UpdateAllStitchesOfPlushie();
    }
    void RepositionCameras()
    {
        var cameraHandler = ServiceLocator.GetService<ICameraManager>();
        if(cameraHandler != null )
        {
            cameraHandler.RepositionCamera(cameraHandler.neckCamera.transform, levelScriptable.neckCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.leftEyeCamera.transform, levelScriptable.leftEyeCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.leftEarCamera.transform, levelScriptable.leftEarCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.rightEarCamera.transform, levelScriptable.rightEarCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.rightEyeCamera.transform, levelScriptable.rightEyeCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.rightArmCamera.transform, levelScriptable.rightArmCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.rightLegCamera.transform, levelScriptable.rightLegCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.leftLegCamera.transform, levelScriptable.leftLegCameraPos);
            cameraHandler.RepositionCamera(cameraHandler.leftArmCamera.transform, levelScriptable.leftArmCameraPos);
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
    void DisableAllSewPoints()
    {
        GetAllPartsObjects(bodyParts);
        GetAllPartsObjects(head.joints);
        Part_Info bodyPartInfo = immoveablePart.GetComponent<Part_Info>();
        if (bodyPartInfo) GetAllPartsObjects(bodyPartInfo.joints);
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
        current_ObjectInfor = stitchStartingPart;
        if (stitchStartingPart.stitchData.IsStitched)
        {
            current_ObjectInfor = GetObjectInfoOfCurrentUnstitchedPart(bodyParts);
        }
        currentActivePart = current_ObjectInfor.partType;
        NextPartActivation(current_ObjectInfor);

        //ObjectInfo currentConnectedPartInfor = null;
        //ObjectInfo neck = bodyParts[0].GetComponent<ObjectInfo>();
        //if (neck.stitchData.IsStitched)
        //    current_ObjectInfor = GetObjectInfoOfCurrentUnstitchedPart(levelDivision.rightSide);
        //else
        //    current_ObjectInfor = neck;

        //if (current_ObjectInfor != null)
        //{
        //    levelDivision.rightSideIndex = levelDivision.rightSide.IndexOf(current_ObjectInfor.gameObject) + 1;
        //    if (current_ObjectInfor.partConnectedTo.Equals(PartConnectedTo.head))
        //        currentConnectedPartInfor = GetObjectInfoOfCurrentUnstitchedPart(head.joints);
        //    else
        //        currentConnectedPartInfor = GetObjectInfoOfCurrentUnstitchedPart(immoveablePart.GetComponent<Part_Info>().joints);
        //    NextPartActivation(/*true,*/currentConnectedPartInfor);
        //}
        //else
        //    NextPartActivation(/*true,*/ null);

    }
    void NextPartActivation(/*bool start,*/ObjectInfo currentActivePart)
    {
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
            needleDetecto.detect = false;

        Part_Info p2_Info = immoveablePart.GetComponent<Part_Info>();
        //sequenceType = SequenceType.right;

        //if (!start)
        //{
        //    if (levelDivision.rightSideIndex < levelDivision.rightSide.Count)
        //        current_ObjectInfor = levelDivision.rightSide[levelDivision.rightSideIndex].GetComponent<ObjectInfo>();

        //    levelDivision.rightSideIndex++;
        //}
        //if(current_ObjectInfor == null)
        //    current_ObjectInfor = bodyParts[0].GetComponent<ObjectInfo>();

        EnableDisableSewPoints(currentActivePart.connectPoints, true);
        if (currentActivePart.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(currentActivePart.partType, true);

        if (currentActivePart.partConnectedTo.Equals(PartConnectedTo.head))
            head.EnableJoint(currentActivePart.partType, true);

        CameraFocus(currentActivePart.partType);
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
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler != null)
        {
            if (gameHandler.saveProgress)
            {
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
                        if ((i + 1) < pointsHandler.points.Count)
                        {
                            Connections newConnection = new Connections(pointsHandler.points[i].transform, pointsHandler.points[i + 1].transform, pointsHandler.linePrefab, -0.01f, false, 0);
                            pointsHandler.connections.Add(newConnection);
                        }
                    }
                }
            }
        }
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
        {
            needleDetecto.detect = true;
        }
       
        var needleDetector = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetector != null)
        {
            if (needleDetector.pointsDetected.Count > 0)
                needleDetector.ResetDetectedPointsList(needleDetector.pointsDetected.OrderBy(p => p.attachmentId).ToList());
        }
        CancelInvoke("EnableDetection");
    }
 
    public void UpdateLevelProgress(/*SequenceType sequence*/)
    {
        var canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasManager != null)
            canvasManager.UpdatePlushieStitchProgress(levelScriptable.totalParts, noOfStitchedPart);
        var IthreadHandler = ServiceLocator.GetService<IThreadManager>();
        
        if (noOfStitchedPart.Equals(levelScriptable.totalParts))
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
            //NextPartActivation(/*false, */null);
            NextPartActivation(current_ObjectInfor);
        }
    }
    void WinEffect()
    {
        Time.timeScale = 1.0f;
        GameEvents.GameCompleteEvents.onPlushieComplete.RaiseEvent();
        Invoke(nameof(DisableLevel), 0.5f);
        CancelInvoke(nameof(WinEffect));
    }
    void DisableLevel()
    {
        sewnPlushie.SetActive(true);
        gameObject.SetActive(false);
        CancelInvoke(nameof(DisableLevel));
    }
    void PlaySewnSound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.levelUp;
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
            canvasManager.UpdateStitchCount(levelScriptable.totalStitches, noOfStitchesDone);
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
                    this.currentActivePart = PlushieActiveStitchPart.neck;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.neckCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.righteye:
                    this.currentActivePart = PlushieActiveStitchPart.righteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightEyeCamera);
                    if(needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.32f);
                        needleHandler.ChangeMinThreshHoldValue(1.30e-05f);

                    }
                    break;
                case PlushieActiveStitchPart.lefteye:
                    this.currentActivePart = PlushieActiveStitchPart.lefteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftEyeCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.32f);
                        needleHandler.ChangeMinThreshHoldValue(1.30e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightear:
                    this.currentActivePart = PlushieActiveStitchPart.rightear;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightEarCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftear:
                    this.currentActivePart = PlushieActiveStitchPart.leftear;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftEarCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightarm:
                    this.currentActivePart = PlushieActiveStitchPart.rightarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightArmCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftarm:
                    this.currentActivePart = PlushieActiveStitchPart.leftarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftArmCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightleg:
                    this.currentActivePart = PlushieActiveStitchPart.rightleg;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightLegCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftleg:
                    this.currentActivePart = PlushieActiveStitchPart.leftleg;
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
                        noOfStitchesDone -= sewPointHandler.connections.Count;
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
        noOfStitchesDone = 0;
    }
    public void CheckIfStitchedBeforeCompleteScreen()
    {
        if (noOfStitchedPart.Equals(levelScriptable.totalParts))
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
    public void Connection(SewPoint sp1, SewPoint sp2)
    {
        Vector3 pos1 = Vector3.zero; Vector3 pos2 = Vector3.zero;
        if (sp1.cleanStitchPoint != null && sp2.cleanStitchPoint != null)
        {
            this.lineForCleanConnection = GameObject.Instantiate(linePrefabForCleanConnection);

            pos1 = sp1.cleanStitchPoint.position;
            pos2 = sp2.cleanStitchPoint.position;
            this.lineForCleanConnection.positionCount = 2;
            pos1.z = -0.01f;
            pos2.z = -0.01f;
            this.lineForCleanConnection.SetPosition(0, pos1);
            this.lineForCleanConnection.SetPosition(1, pos2);
            this.lineForCleanConnection.material.color = levelScriptable.threadColor;
            Connections connection = new Connections(sp1.cleanStitchPoint, sp2.cleanStitchPoint, linePrefabForCleanConnection, -0.01f, false, 2);
            cleanConnection.Add(connection);
            connection.line.gameObject.SetActive(false);
            connection.isLocked = true;
            cleanThreads.Add(connection);
            Debug.LogError("connection");
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
