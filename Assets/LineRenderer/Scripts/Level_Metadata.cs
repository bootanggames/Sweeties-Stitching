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
    private void Start()
    {
        //NextPartActivation();
    }

    void NextPartActivation()
    {
        Part_Info p1_Info = head.GetComponent<Part_Info>();
        DisablePartsOfPartInfoType(p1_Info);
        Part_Info p2_Info = immoveablePart.GetComponent<Part_Info>();
        DisablePartsOfPartInfoType(p2_Info);
        foreach (GameObject g in levelParts)
        {
            g.SetActive(false);
        }
        levelParts[partIndex].SetActive(true);
        ObjectInfo o_info = levelParts[partIndex].GetComponent<ObjectInfo>();
        if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(o_info.partType);
        if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
            p1_Info.EnableJoint(o_info.partType);

        CameraFocus(o_info.partType);

        partIndex++;
    }

    void DisablePartsOfPartInfoType(Part_Info p_Info)
    {
        foreach (GameObject p in p_Info.joints)
        {
            p.SetActive(false);
        }
    }
    public void UpdateLevelProgress()
    {
        noOfStitchedPart++;
        if (noOfStitchedPart.Equals(totalStitchedPart))
        {
            GameEvents.GameCompleteEvents.onGameWin.RaiseEvent();
            Invoke("CallGameWinPanel", 1.5f);

        }
        else
        {
            NextPartActivation();
        }
    }
    void CallGameWinPanel()
    {
        GameEvents.GameCompleteEvents.onGameComplete.RaiseEvent();
        CancelInvoke("CallGameWinPanel");
    }
    void CameraFocus(PlushieActiveStitchPart currentActivePart)
    {
        switch(currentActivePart)
        {
            case PlushieActiveStitchPart.neck:
                plushieActivePartToStitch = PlushieActiveStitchPart.neck;
                Part_Info p = levelParts[0].GetComponent<Part_Info>();
                ObjectInfo o = p.joints[0].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o.targetCameraPoint;
                cam.Target.LookAtTarget = o.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.righteye:
                plushieActivePartToStitch = PlushieActiveStitchPart.righteye;
                ObjectInfo o_eyeRight = levelParts[6].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_eyeRight.targetCameraPoint;
                cam.Target.LookAtTarget = o_eyeRight.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.lefteye:
                plushieActivePartToStitch = PlushieActiveStitchPart.lefteye;
                ObjectInfo o_eyeLeft = levelParts[5].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_eyeLeft.targetCameraPoint;
                cam.Target.LookAtTarget = o_eyeLeft.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.rightear:
                plushieActivePartToStitch = PlushieActiveStitchPart.rightear;
                ObjectInfo o_earRight = levelParts[7].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_earRight.targetCameraPoint;
                cam.Target.LookAtTarget = o_earRight.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.leftear:
                plushieActivePartToStitch = PlushieActiveStitchPart.leftear;
                ObjectInfo o_earLeft= levelParts[8].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_earLeft.targetCameraPoint;
                cam.Target.LookAtTarget = o_earLeft.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.rightarm:
                plushieActivePartToStitch = PlushieActiveStitchPart.rightarm;
                ObjectInfo o_armRight = levelParts[2].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_armRight.targetCameraPoint;
                cam.Target.LookAtTarget = o_armRight.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.leftarm:
                plushieActivePartToStitch = PlushieActiveStitchPart.leftarm;
                ObjectInfo o_armLeft = levelParts[1].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_armLeft.targetCameraPoint;
                cam.Target.LookAtTarget = o_armLeft.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.rightleg:
                plushieActivePartToStitch = PlushieActiveStitchPart.rightleg;
                ObjectInfo o_rightLeg = levelParts[3].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_rightLeg.targetCameraPoint;
                cam.Target.LookAtTarget = o_rightLeg.targetCameraPoint;
                break;
            case PlushieActiveStitchPart.leftleg:
                plushieActivePartToStitch = PlushieActiveStitchPart.leftleg;
                ObjectInfo o_leftLeg = levelParts[4].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o_leftLeg.targetCameraPoint;
                cam.Target.LookAtTarget = o_leftLeg.targetCameraPoint;
                break;
        }
    }
}
