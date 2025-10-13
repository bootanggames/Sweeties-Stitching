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
    private void Start()
    {
        //Invoke("StartLevel", 2.0f);
    }
    public void StartLevel() 
    {
        NextPartActivation(true);
        //CancelInvoke("StartLevel");
    }
    void NextPartActivation(bool start)
    {
        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if (needleDetecto != null)
        {
            needleDetecto.detect = false;
        }
        Part_Info p1_Info = head.GetComponent<Part_Info>();
        DisablePartsOfPartInfoType(p1_Info);
        Part_Info p2_Info = immoveablePart.GetComponent<Part_Info>();
        DisablePartsOfPartInfoType(p2_Info);
        foreach (GameObject g in levelParts)
        {
            ObjectInfo objectInfo = g.GetComponent<ObjectInfo>();
            if(!objectInfo.IsStitched)
                g.SetActive(false);
        }
        ObjectInfo o_info = null;
        if (!start)
        {
            if (partIndex == 1)
            {
                SewPoint sp = null;
                IThreadManager threadManager = ServiceLocator.GetService<IThreadManager>();
                if (threadManager != null)
                {
                    sp = threadManager.detectedPoints[threadManager.detectedPoints.Count - 1].GetComponent<SewPoint>();
                    //Debug.LogError(" "+sp.sequenceType.ToString());
                    sp.name = sp.sequenceType.ToString();
                    if (sp.sequenceType.Equals(SequenceType.left))
                        sequenceType = SequenceType.left;
                    else
                        sequenceType = SequenceType.right;
                }
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
            p1_Info.joints[0].SetActive(true);
            o_info = p2_Info.joints[partIndex].GetComponent<ObjectInfo>();
        }
        if (o_info.partConnectedTo.Equals(PartConnectedTo.body))
            p2_Info.EnableJoint(o_info.partType);
        if (o_info.partConnectedTo.Equals(PartConnectedTo.head))
            p1_Info.EnableJoint(o_info.partType);
        o_info.gameObject.SetActive(true);
        CameraFocus(o_info.partType);
        Invoke("EnableDetection", 2.5f);
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
        foreach (GameObject p in p_Info.joints)
        {
            ObjectInfo objectInfo = p.GetComponentInChildren<ObjectInfo>();
            if (objectInfo)
            {
                objectInfo.gameObject.SetActive(true);
                if (!objectInfo.IsStitched)
                    p.SetActive(false);
            }
      
        }
    }
    public void UpdateLevelProgress()
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
            cam.Target.TrackingTarget = levelCompleteView;
            cam.Target.LookAtTarget = levelCompleteView;
            cam.Lens.OrthographicSize = 3;
            Invoke("WinEffect", 2.0f);
        }
        else
        {
            NextPartActivation(false);
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
        switch(currentActivePart)
        {
            case PlushieActiveStitchPart.neck:
                plushieActivePartToStitch = PlushieActiveStitchPart.neck;
                ObjectInfo o = head.joints[0].GetComponent<ObjectInfo>();
                cam.Target.TrackingTarget = o.targetCameraPoint;
                cam.Target.LookAtTarget = o.targetCameraPoint;
                cam.Lens.OrthographicSize = 0.5f;
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
