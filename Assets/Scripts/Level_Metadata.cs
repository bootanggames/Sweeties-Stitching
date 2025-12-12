using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level_Metadata : MonoBehaviour
{
    public bool completed = false;
    public LevelDataScriptable levelScriptable;
    public int noOfStitchesDone;
    public int noOfStitchedPart;
    public int currentActiveSpoolIndex;
    public PlushieActiveStitchPart currentActivePart;
    public List<GameObject> bodyParts;
    public Part_Info head;
    public GameObject immoveablePart;
    public GameObject bodyWihtoutHoles;
    public GameObject sewnPlushie;
    LineRenderer lineForCleanConnection;
    [SerializeField] LineRenderer linePrefabForCleanConnection;
    [SerializeField] ObjectInfo stitchStartingPart;
    [HideInInspector] public ObjectInfo current_ObjectInfor = null;
    [HideInInspector] public Transform needleUndoPosition;
    [HideInInspector] public List<Connections> cleanConnection;
    [HideInInspector] public int cleanThreadIndex = 0;
    [HideInInspector] public List<GameObject> crissCrossObjList = new List<GameObject>();
    List<Connections> cleanThreads = new List<Connections>();
    [HideInInspector]public GameObject currentSpool;
    ISpoolManager spoolManager;
    IThreadManager threadHandler;
    INeedleDetector needleDetecto;
    ICameraManager cameraManager;
    IPointConnectionHandler pointsHandler;
    ICanvasUIManager canvasManager;
    INeedleMovement needleHandler;
    private void Start()
    {
        Time.timeScale = 1;
        spoolManager  = ServiceLocator.GetService<ISpoolManager>();
        threadHandler = ServiceLocator.GetService<IThreadManager>();
        needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        cameraManager = ServiceLocator.GetService<ICameraManager>();
        pointsHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
        needleHandler = ServiceLocator.GetService<INeedleMovement>();

        AssignAndUpdateSpools();
        LevelInitialisation();

    }

    void AssignAndUpdateSpools()
    {
        
        if (spoolManager != null)
        {
            spoolManager.SpoolList(levelScriptable.totalSpoolsNeeded);
            //canvasHandler.spoolImg.sprite = levelScriptable.threadSpool;
            spoolManager.ChangeSpriteOfSpools(levelScriptable.threadSpool);
            currentSpool = spoolManager.GetSpool(currentActiveSpoolIndex);
            SpoolInfo s_Info = currentSpool.GetComponent<SpoolInfo>();
            needleUndoPosition = s_Info.undoPosition;
            int totalThread = 0;
            if (levelScriptable.totalSpoolsNeeded > 1)
            {
                float total = (levelScriptable.totalStitches / spoolManager.spoolList.Count);
                //totalThread = (float)Math.Ceiling(total) + 1;
                totalThread = (int)total;
            }
            else
                totalThread = levelScriptable.totalStitches;
            if (GameHandler.instance.saveProgress)
            {
                LoadSpoolDataIfSaved(spoolManager);
            }

            s_Info.UpdateThreadProgress(totalThread);

        }
        if (threadHandler != null)
            threadHandler.UpdateCurrentActiveSpoolReference();
    }
    void LoadSpoolDataIfSaved(ISpoolManager spoolManager)
    {
        if (SaveDataUsingJson.instance)
        {
            int levelIndex = LevelsHandler.instance.levelIndex;
            string _plushieName = LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName;
            for(int i=0;i< spoolManager.spoolList.Count; i++)
            {
                SpoolInfo s_Info = spoolManager.spoolList[i].GetComponent<SpoolInfo>();
                s_Info._spoolData = SaveDataUsingJson.instance.LoadData<SpoolData>(s_Info._spoolData.spoolId + "_" + levelIndex + "_" + _plushieName, "SpoolData");
                if (s_Info._spoolData != null)
                    s_Info.UpdateThreadProgress(s_Info._spoolData.totalThreadsInSpool);
            }
          
        }
    }
    public void LevelInitialisation()
    {
        DisableAllSewPoints();
        RepositionCameras();
        UpdateAllStitchesOfPlushie();
    }
    void RepositionCameras()
    {
        if(cameraManager != null )
        {
            cameraManager.RepositionCamera(cameraManager.neckCamera.transform, levelScriptable.neckCameraPos);
            cameraManager.RepositionCamera(cameraManager.leftEyeCamera.transform, levelScriptable.leftEyeCameraPos);
            cameraManager.RepositionCamera(cameraManager.leftEarCamera.transform, levelScriptable.leftEarCameraPos);
            cameraManager.RepositionCamera(cameraManager.rightEarCamera.transform, levelScriptable.rightEarCameraPos);
            cameraManager.RepositionCamera(cameraManager.rightEyeCamera.transform, levelScriptable.rightEyeCameraPos);
            cameraManager.RepositionCamera(cameraManager.rightArmCamera.transform, levelScriptable.rightArmCameraPos);
            cameraManager.RepositionCamera(cameraManager.rightLegCamera.transform, levelScriptable.rightLegCameraPos);
            cameraManager.RepositionCamera(cameraManager.leftLegCamera.transform, levelScriptable.leftLegCameraPos);
            cameraManager.RepositionCamera(cameraManager.leftArmCamera.transform, levelScriptable.leftArmCameraPos);
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
        Time.timeScale = 1;

        current_ObjectInfor = stitchStartingPart;
        if (stitchStartingPart.stitchData.IsStitched)
        {
            current_ObjectInfor = GetObjectInfoOfCurrentUnstitchedPart(bodyParts);
        }
        currentActivePart = current_ObjectInfor.partType;
        NextPartActivation(current_ObjectInfor);
    }
    void NextPartActivation(/*bool start,*/ObjectInfo currentActivePart)
    {
        if (needleDetecto != null)
            needleDetecto.detect = false;

        Part_Info p2_Info = immoveablePart.GetComponent<Part_Info>();

        EnableDisableSewPoints(currentActivePart.connectPoints, true);
        if (currentActivePart.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(currentActivePart.partType, true);

        if (currentActivePart.partConnectedTo.Equals(PartConnectedTo.head))
            head.EnableJoint(currentActivePart.partType, true);

        CameraFocus(currentActivePart.partType);
        Invoke("EnableDetection", 0.22f);
    }

    IEnumerator CheckProgress(ObjectInfo o_info)
    {
        if (cameraManager != null)
            GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.gameHalfProgressCamera);
        yield return new WaitForSeconds(2);
        CameraFocus(o_info.partType);
        StopCoroutine(CheckProgress(o_info));
    }
    void EnableDetection()
    {
        if (GameHandler.instance != null)
        {
            if (GameHandler.instance.saveProgress)
            {
                if (threadHandler != null)
                {
                    threadHandler.ResetList(threadHandler.detectedPoints.OrderBy(t => t.GetComponent<SewPoint>().attachmentId).ToList());
                }
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
       
        if (needleDetecto != null)
        {
            needleDetecto.detect = true;

            if (needleDetecto.pointsDetected.Count > 0)
                needleDetecto.ResetDetectedPointsList(needleDetecto.pointsDetected.OrderBy(p => p.attachmentId).ToList());
        }
        CancelInvoke("EnableDetection");
    }
 
    public void UpdateLevelProgress(/*SequenceType sequence*/)
    {

        if (canvasManager != null)
            canvasManager.UpdatePlushieStitchProgress(levelScriptable.totalParts, noOfStitchedPart);
        
        if (noOfStitchedPart.Equals(levelScriptable.totalParts))
        {
            Time.timeScale = 1.2f;
            //PlaySewnSound();

            if (cameraManager != null)
                GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.gameCompleteCamera);

            if (bodyWihtoutHoles)
            {
                bodyWihtoutHoles.SetActive(true);
                immoveablePart.GetComponent<SpriteRenderer>().enabled = false;
            }
            
            GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);
           if(threadHandler != null)
                threadHandler.SetUndoValue(false);
            //WinEffect();
            Invoke("WinEffect",0.5f);
        }
        else
        {
            if (threadHandler != null)
                threadHandler.SetUndoValue(true);
            //NextPartActivation(/*false, */null);
            NextPartActivation(current_ObjectInfor);
        }
    }
    void WinEffect()
    {
        Time.timeScale = 1.0f;
        GameEvents.GameCompleteEvents.onPlushieComplete.Raise();
        HepticManager.instance.HapticEffect();
        Invoke(nameof(DisableLevel), 0.5f);
        CancelInvoke(nameof(WinEffect));
    }
    void DisableLevel()
    {
        sewnPlushie.SetActive(true);
        gameObject.SetActive(false);
        CancelInvoke(nameof(DisableLevel));
    }
    public void PlaySewnSound()
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
        GameEvents.GameCompleteEvents.onGameComplete.Raise();
        CancelInvoke("CallGameWinPanel");
    }
  
    public void UpdateAllStitchesOfPlushie()
    {
        if (canvasManager != null)
            canvasManager.UpdateStitchCount(levelScriptable.totalStitches, noOfStitchesDone);
    }
    public void CameraFocus(PlushieActiveStitchPart currentActivePart)
    {
        if (cameraManager != null)
        {
            switch (currentActivePart)
            {
                case PlushieActiveStitchPart.neck:
                    this.currentActivePart = PlushieActiveStitchPart.neck;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.neckCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.righteye:
                    this.currentActivePart = PlushieActiveStitchPart.righteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.rightEyeCamera);
                    if(needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.32f);
                        needleHandler.ChangeMinThreshHoldValue(1.30e-05f);

                    }
                    break;
                case PlushieActiveStitchPart.lefteye:
                    this.currentActivePart = PlushieActiveStitchPart.lefteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.leftEyeCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.32f);
                        needleHandler.ChangeMinThreshHoldValue(1.30e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightear:
                    this.currentActivePart = PlushieActiveStitchPart.rightear;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.rightEarCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftear:
                    this.currentActivePart = PlushieActiveStitchPart.leftear;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.leftEarCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightarm:
                    this.currentActivePart = PlushieActiveStitchPart.rightarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.rightArmCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftarm:
                    this.currentActivePart = PlushieActiveStitchPart.leftarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.leftArmCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.rightleg:
                    this.currentActivePart = PlushieActiveStitchPart.rightleg;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.rightLegCamera);
                    if (needleHandler != null)
                    {
                        needleHandler.NeedleSize(0.4f);
                        needleHandler.ChangeMinThreshHoldValue(1.33e-05f);
                    }
                    break;
                case PlushieActiveStitchPart.leftleg:
                    this.currentActivePart = PlushieActiveStitchPart.leftleg;
                    GameEvents.CameraManagerEvents.onAddingCamera.Raise(cameraManager.leftLegCamera);
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
        if( pointsHandler != null)
        {
            foreach (GameObject g in bodyParts)
            {
                ObjectInfo ob_info = g.GetComponent<ObjectInfo>();
                if (!ob_info.stitchData.IsStitched)
                {
                    if (ob_info.Equals(current_ObjectInfor))
                    {
                        noOfStitchesDone -= pointsHandler.connections.Count;
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
        Vector3 threadPos = Vector3.zero;

        if (threadHandler != null && threadHandler.instantiatedLine != null)
        {
            position.z = threadHandler.zVal;
            GameEvents.NeedleEvents.OnNeedleMovement.Raise(position);
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
