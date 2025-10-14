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

    public CinemachineCamera cam;
    [SerializeField] LevelDivision levelDivision;
    [SerializeField] SequenceType sequenceType;
    [SerializeField] Transform levelCompleteView;
  
    public void StartLevel() 
    {
        NextPartActivation(true, SequenceType.none);
    }
    void NextPartActivation(bool start, SequenceType sequence)
    {
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
        {
            needleDetecto.detect = false;
        }
        Part_Info p1_Info = head.GetComponent<Part_Info>();
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
                levelDivision.leftSideIndex++;
            }
            else
            {
                o_info = levelDivision.rightSide[levelDivision.rightSideIndex].GetComponent<ObjectInfo>();
                levelDivision.rightSideIndex++;
            }
        }
        else
        {
            //p1_Info.joints[0].SetActive(true);
            o_info = p2_Info.joints[partIndex].GetComponent<ObjectInfo>();
        }
        if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(o_info.partType);
        if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
            p1_Info.EnableJoint(o_info.partType);
        o_info.gameObject.SetActive(true);
        CameraFocus(o_info.partType);
        Invoke("EnableDetection", 0.15f);
        partIndex++;
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
    void DisablePartsOfPartInfoType(Part_Info p_Info)
    {
        //foreach (GameObject p in p_Info.joints)
        //{
        //    ObjectInfo objectInfo = p.GetComponentInChildren<ObjectInfo>();
        //    if (objectInfo)
        //    {
        //        objectInfo.gameObject.SetActive(true);
        //        if (!objectInfo.IsStitched)
        //            p.SetActive(false);
        //    }
      
        //}
    }
    public void UpdateLevelProgress(SequenceType sequence)
    {
        noOfStitchedPart++;
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
