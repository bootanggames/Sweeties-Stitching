using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Metadata : MonoBehaviour
{
    public int levelReward = 0;
    public int totalCorrectLinks;
    public int noOfLinks;
    public PlushieActiveStitchPart plushieActivePartToStitch;
    public List<GameObject> bodyParts;
    int partIndex = 0;
    public Part_Info head;
    public GameObject immoveablePart;
    public GameObject bodyWihtoutHoles;

    public int totalStitchedPart;
    public int noOfStitchedPart;

    [SerializeField] LevelDivision levelDivision;
    [SerializeField] SequenceType sequenceType;

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

    public Sprite plushieSprite;
    private void Start()
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
            s.gameObject.SetActive(val);
        }
    }
    void GetAllPartsObjects(List<GameObject> parts)
    {
        foreach (GameObject g in parts)
        {
            ObjectInfo o = g.GetComponent<ObjectInfo>();
            EnableDisableSewPoints(o.connectPoints, false);
        }
    }
    void HandlerPointsEnableDisable()
    {
        GetAllPartsObjects(bodyParts);
        GetAllPartsObjects(head.joints);
        GetAllPartsObjects(immoveablePart.GetComponent<Part_Info>().joints);
    }
    public ObjectInfo GetObjectInfoOfCurrentUnstitchedPart(List<GameObject> list)
    {
        ObjectInfo ob_info = null;
        foreach (GameObject g in list)
        {
            ObjectInfo o = g.GetComponent<ObjectInfo>();
            if (!o.IsStitched)
            {
                ob_info = o;
                break;
            }
           
        }
        return ob_info;
    }
    public void StartLevel() 
    {
        ObjectInfo currentPartInfor = null;
        ObjectInfo currentConnectedPartInfor = null;
        ObjectInfo neck = bodyParts[0].GetComponent<ObjectInfo>();
        if (neck.IsStitched)
            currentPartInfor = GetObjectInfoOfCurrentUnstitchedPart(levelDivision.rightSide);
        else
            currentPartInfor = neck;
        if (currentPartInfor != null)
        {
            levelDivision.rightSideIndex = levelDivision.rightSide.IndexOf(currentPartInfor.gameObject) + 1;
            if (currentPartInfor.partConnectedTo.Equals(PartConnectedTo.head))
                currentConnectedPartInfor = GetObjectInfoOfCurrentUnstitchedPart(head.joints);
            else
                currentConnectedPartInfor = GetObjectInfoOfCurrentUnstitchedPart(immoveablePart.GetComponent<Part_Info>().joints);
            NextPartActivation(true, SequenceType.none, currentPartInfor, currentConnectedPartInfor);
        }
        else
            NextPartActivation(true, SequenceType.none, null, null);

    }
    ObjectInfo current_ObjectInfor = null;

    void NextPartActivation(bool start, SequenceType sequence, ObjectInfo currentPart, ObjectInfo connectedTo)
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
        else
        {
            if(currentPart == null && connectedTo == null)
                current_ObjectInfor = bodyParts[0].GetComponent<ObjectInfo>();
            else
                current_ObjectInfor = currentPart;
        }
        EnableDisableSewPoints(current_ObjectInfor.connectPoints, true);
        if (current_ObjectInfor.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(current_ObjectInfor.partType, true);

        if (current_ObjectInfor.partConnectedTo.Equals(PartConnectedTo.head))
            head.EnableJoint(current_ObjectInfor.partType, true);

        CameraFocus(current_ObjectInfor.partType);
        Invoke("EnableDetection", 0.15f);
        partIndex++;
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
        CancelInvoke("EnableDetection");
    }
 
    public void UpdateLevelProgress(SequenceType sequence)
    {
        var canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasManager != null)
            canvasManager.UpdatePlushieStitchProgress(totalStitchedPart, noOfStitchedPart);
        
        if (noOfStitchedPart.Equals(totalStitchedPart))
        {
            var cameraManager = ServiceLocator.GetService<ICameraManager>();
            if (cameraManager != null)
                GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.gameCompleteCamera);

            if (bodyWihtoutHoles)
            {
                bodyWihtoutHoles.SetActive(true);
                immoveablePart.GetComponent<SpriteRenderer>().enabled = false;
            }
            GameHandler.instance.SwitchGameState(GameStates.Gamecomplete);

            Invoke("WinEffect", 2.0f);
        }
        else
        {
          
            NextPartActivation(false, sequence, null, null);
        }
    }
    void WinEffect()
    {
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if(canvasHandler != null)
            canvasHandler.sewnScreen.SetActive(true);
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
        if (cameraManager != null)
        {
            switch (currentActivePart)
            {
                case PlushieActiveStitchPart.neck:
                    plushieActivePartToStitch = PlushieActiveStitchPart.neck;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.neckCamera);
                    break;
                case PlushieActiveStitchPart.righteye:
                    plushieActivePartToStitch = PlushieActiveStitchPart.righteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightEyeCamera);

                    break;
                case PlushieActiveStitchPart.lefteye:
                    plushieActivePartToStitch = PlushieActiveStitchPart.lefteye;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftEyeCamera);

                    break;
                case PlushieActiveStitchPart.rightear:
                    plushieActivePartToStitch = PlushieActiveStitchPart.rightear;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightEarCamera);

                    break;
                case PlushieActiveStitchPart.leftear:
                    plushieActivePartToStitch = PlushieActiveStitchPart.leftear;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftEarCamera);

                    break;
                case PlushieActiveStitchPart.rightarm:
                    plushieActivePartToStitch = PlushieActiveStitchPart.rightarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightArmCamera);

                    break;
                case PlushieActiveStitchPart.leftarm:
                    plushieActivePartToStitch = PlushieActiveStitchPart.leftarm;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftArmCamera);

                    break;
                case PlushieActiveStitchPart.rightleg:
                    plushieActivePartToStitch = PlushieActiveStitchPart.rightleg;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.rightLegCamera);

                    break;
                case PlushieActiveStitchPart.leftleg:
                    plushieActivePartToStitch = PlushieActiveStitchPart.leftleg;
                    GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.leftLegCamera);

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
                if (!ob_info.IsStitched)
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
            if (!o_Info.head)
                o_Info.ResetPart();
            else
            {
                head.transform.position = o_Info.startPosition;
                o_Info.ResetPart();
            }
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

            int leveCount = PlayerPrefs.GetInt("Level");
            PlayerPrefs.DeleteAll();

            leveCount++;

            if (leveCount >= LevelsHandler.instance.levels.Count)
                leveCount = 0;
            Level_Metadata nextLevel = LevelsHandler.instance.levels[leveCount].GetComponent<Level_Metadata>();
            nextLevel.ResetLevel();
            LevelsHandler.instance.SetPref(leveCount);
            LevelsHandler.instance.SetLevel();
        }
    }
}
