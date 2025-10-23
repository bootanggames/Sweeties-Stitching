using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class Level_Metadata : MonoBehaviour
{
    public int totalCorrectLinks;
    public int noOfCorrectLinks;
    public PlushieActiveStitchPart plushieActivePartToStitch;
    public List<GameObject> levelParts;
    int partIndex = 0;
    public Part_Info head;
    public GameObject immoveablePart;

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
    public void StartLevel() 
    {
        //HandlerPointsEnableDisable();
        NextPartActivation(true, SequenceType.none);
    }
    void NextPartActivation(bool start, SequenceType sequence)
    {
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
        {
            needleDetecto.detect = false;
        }

        Part_Info p2_Info = immoveablePart.GetComponent<Part_Info>();
        ObjectInfo o_info = null;
        if (!start)
        {
            if (partIndex == 1)
            {
                if (sequence.Equals(SequenceType.left))
                    sequenceType = SequenceType.left;
                else
                    sequenceType = SequenceType.right;
            }
            if (sequenceType.Equals(SequenceType.left))
            {
                if (levelDivision.leftSideIndex >= levelDivision.leftSide.Count) return;
                o_info = levelDivision.leftSide[levelDivision.leftSideIndex].GetComponent<ObjectInfo>();
                EnableDisableSewPoints(o_info.connectPoints, true);
                if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
                    p2_Info.EnableJoint(o_info.partType,true);
                if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
                    head.EnableJoint(o_info.partType,true);
                levelDivision.leftSideIndex++;
            }
            else
            {
                o_info = levelDivision.rightSide[levelDivision.rightSideIndex].GetComponent<ObjectInfo>();
                EnableDisableSewPoints(o_info.connectPoints, true);
                if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
                    p2_Info.EnableJoint(o_info.partType, true);
                if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
                    head.EnableJoint(o_info.partType, true);
                levelDivision.rightSideIndex++;
            }
        }
        else
        {
            o_info = p2_Info.joints[partIndex].GetComponent<ObjectInfo>();
            EnableDisableSewPoints(o_info.connectPoints, true);
            EnableDisableSewPoints(head.joints[partIndex].GetComponent<ObjectInfo>().connectPoints, true);
        }

        //------for half progress camera enable-------

        //float percent = 0;
        //if (totalStitchedPart == 0)
        //    percent = 0;
        //else
        //    percent = ((float)noOfStitchedPart / totalStitchedPart) * 100;
        //if(Mathf.FloorToInt(percent) == 55)
        //{
        //    StartCoroutine(CheckProgress(o_info));
        //}
        //else 
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
            Invoke("WinEffect", 2.0f);
        }
        else
        {
          
            NextPartActivation(false, sequence);
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
    void CameraFocus(PlushieActiveStitchPart currentActivePart)
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
