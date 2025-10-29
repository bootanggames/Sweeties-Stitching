using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Level_Metadata : MonoBehaviour
{
    public int levelReward = 0;
    public int totalCorrectLinks;
    public int noOfCorrectLinks;
    public PlushieActiveStitchPart plushieActivePartToStitch;
    public List<GameObject> levelParts;
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
        GetAllPartsObjects(levelParts);
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
        ObjectInfo neck = levelParts[0].GetComponent<ObjectInfo>();
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
    void NextPartActivation(bool start, SequenceType sequence, ObjectInfo currentPart, ObjectInfo connectedTo)
    {
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
        {
            needleDetecto.detect = false;
        }

        Part_Info p2_Info = immoveablePart.GetComponent<Part_Info>();
        ObjectInfo o_info = null;
        sequenceType = SequenceType.right;

        if (!start)
        {
            //if (partIndex == 1)
            //{
            //    if (sequence.Equals(SequenceType.left))
            //        sequenceType = SequenceType.left;
            //    else
            //        sequenceType = SequenceType.right;
            //}
            //sequenceType = SequenceType.right;

            //if (sequenceType.Equals(SequenceType.left))
            //{
            //    if (levelDivision.leftSideIndex >= levelDivision.leftSide.Count) return;
            //    o_info = levelDivision.leftSide[levelDivision.leftSideIndex].GetComponent<ObjectInfo>();
            //    EnableDisableSewPoints(o_info.connectPoints, true);
            //    if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
            //        p2_Info.EnableJoint(o_info.partType,true);
            //    if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
            //        head.EnableJoint(o_info.partType,true);
            //    levelDivision.leftSideIndex++;
            //}
            //else
            {
                if (levelDivision.rightSideIndex < levelDivision.rightSide.Count)
                    o_info = levelDivision.rightSide[levelDivision.rightSideIndex].GetComponent<ObjectInfo>();

                //EnableDisableSewPoints(o_info.connectPoints, true);
                //if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
                //    p2_Info.EnableJoint(o_info.partType, true);
                //if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
                //    head.EnableJoint(o_info.partType, true);

                levelDivision.rightSideIndex++;
            }
        }
        else
        {
            if(currentPart == null && connectedTo == null)
            {
                o_info = levelParts[0].GetComponent<ObjectInfo>();
                //EnableDisableSewPoints(head.joints[partIndex].GetComponent<ObjectInfo>().connectPoints, true);
            }
            else
            {
                o_info = currentPart;
                //EnableDisableSewPoints(connectedTo.connectPoints, true);
            }
         
        }
        EnableDisableSewPoints(o_info.connectPoints, true);
        if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(o_info.partType, true);

        if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
            head.EnableJoint(o_info.partType, true);

        CameraFocus(o_info.partType);
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
        noOfStitchedPart++;
      
        var canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasManager != null)
            canvasManager.UpdatePlushieStitchProgress(totalStitchedPart, noOfStitchedPart);
        
        if (noOfStitchedPart.Equals(totalStitchedPart))
        {
            foreach (GameObject g in levelParts)
            {
                g.SetActive(true);
            }
            foreach (GameObject p in head.joints)
            {
                p.SetActive(true);
            }
            var cameraManager = ServiceLocator.GetService<ICameraManager>();
            if (cameraManager != null)
            {
                GameEvents.CameraManagerEvents.onAddingCamera.RaiseEvent(cameraManager.gameCompleteCamera);
            }
            if (bodyWihtoutHoles)
            {
                bodyWihtoutHoles.SetActive(true);
                immoveablePart.GetComponent<SpriteRenderer>().enabled = false;
            }
            Invoke("WinEffect", 2.0f);
        }
        else
        {
          
            NextPartActivation(false, sequence, null, null);
        }
    }
    void WinEffect()
    {
        GameEvents.GameCompleteEvents.onGameWin.RaiseEvent();
       
        Invoke("CallGameWinPanel", 1.5f);
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
            canvasManager.UpdateStitchCount(totalCorrectLinks, noOfCorrectLinks);

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

   
}
