using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointConnectorHandler : MonoBehaviour, IPointConnectionHandler
{
    [field: SerializeField] public List<SewPoint> points { get; private set; }
    [field: SerializeField] public List<Connections> connections { get; private set; }
    [field: SerializeField] public List<SewPoint> wrongConnectPoint { get; private set; }

    [field: SerializeField] public float pullDuration { get; private set; }
    [field: SerializeField] public float minDistance { get; private set; }
    [field: SerializeField] public float maxPullDuration { get; private set; }
    [field: SerializeField] public float minPullDuration { get; private set; }
    [field: SerializeField] public int minThreadStitchCount { get; private set; }
    [field: SerializeField] public int maxThreadStitchCount { get; private set; }
    [field: SerializeField] public int threadStitchCount { get; private set; }
    [field: SerializeField] public bool dynamicStitch { get; private set; }
    [field: SerializeField] public Material correctPointMaterial { get; private set; }
    [field: SerializeField] public Material wrongPointMaterial { get; private set; }
    [field: SerializeField] public Material originalMaterial { get; private set; }
    [field: SerializeField] public Material startToDetectMaterial { get; private set; }

    [SerializeField] float tolerance = 0.05f;
    [field: SerializeField] public LineRenderer linePrefab { get; private set; }
    [SerializeField] float zVal = -0.25f;
    [SerializeField] float rotationSpeed;
    [SerializeField] float pullForce;
    [SerializeField] bool moveStaticallyAtTheEndOfStitch;
    private void OnEnable()
    {

        points = new List<SewPoint>();
        connections = new List<Connections>();
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IPointConnectionHandler>(this);
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.Register(GetAttachedPointsToCreateLink);
        GameEvents.PointConnectionHandlerEvents.onStopTweens.Register(EndTweens);
        GameEvents.PointConnectionHandlerEvents.onUpdatingPullSpeed.Register(SetPullSpeed);
        GameEvents.PointConnectionHandlerEvents.onUpdatingStitchCount.Register(SetStitchCount);
        GameEvents.PointConnectionHandlerEvents.onSettingPlushieLevel2.Register(ActivateDynamicStitch);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPointConnectionHandler>(this);
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.UnRegister(GetAttachedPointsToCreateLink);
        GameEvents.PointConnectionHandlerEvents.onStopTweens.UnRegister(EndTweens);
        GameEvents.PointConnectionHandlerEvents.onUpdatingPullSpeed.UnRegister(SetPullSpeed);
        GameEvents.PointConnectionHandlerEvents.onUpdatingStitchCount.UnRegister(SetStitchCount);
        GameEvents.PointConnectionHandlerEvents.onSettingPlushieLevel2.UnRegister(ActivateDynamicStitch);
    }
    public void ResetPointsList(List<SewPoint> list)
    {
        points = list;
    }
    void ActivateDynamicStitch(bool val)
    {
        dynamicStitch = val;
    }
    void SetPullSpeed(float speed)
    {
        pullDuration = speed;
    }
    void SetStitchCount(int count)
    {
        threadStitchCount = count;
    }
   
    public void UpdateColorOfPoints()
    {
        if (wrongConnectPoint.Count > 0)
        {
            SewPoint lastPoint = wrongConnectPoint[wrongConnectPoint.Count - 1];
            SewPoint secondLast = null;
            if ((wrongConnectPoint.Count - 2) >= 0)
                secondLast = wrongConnectPoint[wrongConnectPoint.Count - 2];

            if (lastPoint == null) return;

            if (lastPoint.metaData == null)
                lastPoint.metaData = new SewPointMetaData();

            if (secondLast != null)
            {
                if (secondLast.metaData == null)
                    secondLast.metaData = new SewPointMetaData();
                if (lastPoint.transform.parent.parent.parent != secondLast.transform.parent.parent.parent)
                {
                    if (secondLast.nextConnectedPointId == lastPoint.attachmentId)
                    {
                        if (secondLast.metaData.connected)
                            lastPoint.pointMesh.material = correctPointMaterial;
                        else
                        {
                            secondLast.pointMesh.material = wrongPointMaterial;
                            lastPoint.pointMesh.material = wrongPointMaterial;
                        }
                    }
                    else
                    {
                        lastPoint.pointMesh.material = wrongPointMaterial;
                        if (secondLast.metaData.connected)
                            secondLast.pointMesh.material = correctPointMaterial;
                    }
                }
                else
                {
                    if (secondLast.nextConnectedPointId != lastPoint.attachmentId)
                    {
                        if (secondLast.metaData.connected)
                            secondLast.pointMesh.material = wrongPointMaterial;
                        lastPoint.pointMesh.material = wrongPointMaterial;
                    }
                    else
                    {
                        if (!secondLast.metaData.connected)
                            secondLast.pointMesh.material = wrongPointMaterial;
                        else
                            secondLast.pointMesh.material = correctPointMaterial;
                    }
                        
                }
            }
            else
            {
                if (lastPoint.startFlag)
                    lastPoint.pointMesh.material = correctPointMaterial;
                else
                {
                    if (points.Count > 0)
                    {
                        SewPoint startPoint = points[0];
                        if (startPoint.startFlag && !startPoint.metaData.connected)
                        {
                            if (startPoint.transform.parent == lastPoint.transform.parent)
                                lastPoint.pointMesh.material = wrongPointMaterial;
                        }

                    }
                }
            }
           
        }
    }
    public void GetAttachedPointsToCreateLink(List<Transform> point)
    {
        SewPoint s_FirstOne = null;

        if (point.Count > 0)
        {
            s_FirstOne = point[0].GetComponent<SewPoint>();
            ObjectInfo ob_Info = s_FirstOne.GetComponentInParent<ObjectInfo>();

            if (!s_FirstOne.startFlag)
            {
                s_FirstOne.pointMesh.material = wrongPointMaterial;
                UpdateWrongSequence(s_FirstOne, ob_Info);
            }
            else
                s_FirstOne.pointMesh.material = correctPointMaterial;
        }
        if (point.Count <= 1) return;
        foreach (Transform t in point)
        {
            if (!points.Contains(t.GetComponent<SewPoint>()))
                points.Add(t.GetComponent<SewPoint>());
        }
        SewPoint sp1 = null;
        SewPoint sp2 = null;
        ObjectInfo o_info1 = null;
        ObjectInfo o_info2 = null;
        sp1 = points[points.Count - 2];
        sp2 = points[points.Count - 1];
        o_info1 = sp1.transform.parent.parent.GetComponent<ObjectInfo>();
        o_info2 = sp2.transform.parent.parent.GetComponent<ObjectInfo>();
        sp2.pointMesh.material = correctPointMaterial;
        if (points.Count > 1)
        {
            if (sp1.transform.parent.parent.parent == sp2.transform.parent.parent.parent)
            {
                if (!sp1.metaData.connected)
                {
                    if (sp1.nextConnectedPointId.Equals(sp2.attachmentId) || !sp1.nextConnectedPointId.Equals(sp2.attachmentId))
                        UpdateWrongSequence(sp2, o_info1);
                }

                if (!sp1.nextConnectedPointId.Equals(sp2.attachmentId))
                    UpdateWrongSequence(sp2, o_info1);
            }
           
            if (!sp1.attachmentId.Equals(sp2.attachmentId))
            {
                if (wrongConnectPoint.Count > 0)
                {
                    if (sp1.nextConnectedPointId.Equals(sp2.attachmentId))
                        UpdateWrongSequence(sp2, o_info1);
                    else
                        UpdateWrongSequence(sp2, o_info1);
                }
                else
                {
                    if (!sp1.metaData.connected)
                        UpdateWrongSequence(sp2, o_info1);
                }

            }
            if (sp1.attachmentId.Equals(sp2.attachmentId))
            {
                if (wrongConnectPoint.Count > 0)
                    UpdateWrongSequence(sp2, o_info1);
            }
            if (sp1.metaData.connected && sp1.nextConnectedPointId != sp2.attachmentId)
                UpdateWrongSequence(sp2, o_info1);

            if (wrongConnectPoint.Count > 0)
            {
                if (!sp1.metaData.connected) sp1.pointMesh.material = wrongPointMaterial;
            }
        }

        if (wrongConnectPoint == null && wrongConnectPoint.Count == 0)
            CheckIfLastConnectionUpdated(sp1, sp2, sp1.transform, sp2.transform, o_info1, o_info2);

        CreateLinkBetweenPoints(sp1, sp2);
    }

    void UpdateWrongSequence(SewPoint sp, ObjectInfo ob_info)
    {
        if (!wrongConnectPoint.Contains(sp))
            wrongConnectPoint.Add(sp);
        sp.pointMesh.material = wrongPointMaterial;

        GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.bodyParts);
        GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.immoveablePart.GetComponent<Part_Info>().joints);
        GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.head.joints);
     
        ob_info.WrongSequenceAlertText("Uh oh! Stitching Pattern is OFF", 2);
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if(canvasHandler != null)
            canvasHandler.undoHighLight.SetActive(true);
        UpdateColorOfPoints();
    }
    public void GetObjectInfoWrongAlertTextDisableOfPart(List<GameObject> list)
    {
        foreach (GameObject g in list)
        {
            ObjectInfo o = g.GetComponent<ObjectInfo>();
            o.DisableWrongAlertText();
        }
    }
    public void CreateLinkBetweenPoints(SewPoint point1, SewPoint point2)
    {
        Connections existingConnection = connections.Find(conn =>
       (conn.point1 == point1.transform && conn.point2 == point2.transform) ||
       (conn.point1 == point2.transform && conn.point2 == point1.transform)
   );

        if (existingConnection != null)
        {
            if (!existingConnection.isLocked)
                ManageConnetions(existingConnection);
            //return;
        }
        if (dynamicStitch)
            NewConnection(point1.transform, point2.transform, true, false, threadStitchCount);
        //else
        //{
        //    if (threadStitchCount == 1)
        //        NewConnection(point1.transform, point2.transform, true, false, threadStitchCount);
        //    else
        //        NewConnection(point1.transform, point2.transform, true, true, threadStitchCount);
        //}
    }

    void NewConnection(Transform p1, Transform p2, bool applyPullForce, bool multiple, int stitchCount)
    {
        if (connections.Exists(c =>
        (c.point1 == p1 && c.point2 == p2) ||
        (c.point1 == p2 && c.point2 == p1)))
            return;
        Connections connection = new Connections(p1, p2, linePrefab, zVal, multiple, stitchCount);
        connections.Add(connection);
        LevelsHandler.instance.currentLevelMeta.noOfStitchesDone++;
        if (LevelsHandler.instance.currentLevelMeta.currentSpool)
        {
            SpoolInfo s_Info = LevelsHandler.instance.currentLevelMeta.currentSpool.GetComponent<SpoolInfo>();
            s_Info._spoolData.noOfStitchedDone++;
        }

        var canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasManager != null)
            canvasManager.UpdateStitchCount(LevelsHandler.instance.currentLevelMeta.levelScriptable.totalStitches, LevelsHandler.instance.currentLevelMeta.noOfStitchesDone);
        if (wrongConnectPoint != null && wrongConnectPoint.Count > 0) return;

        if (applyPullForce)
            ManageConnetions(connection);
        else
            connection.isLocked = true;

    }
    public void ManageConnetions(Connections c)
    {
        SewPoint sp1 = c.point1.GetComponent<SewPoint>();
        SewPoint sp2 = c.point2.GetComponent<SewPoint>();
        if (sp1.attachmentId.Equals(sp2.attachmentId))
            ApplyForces(c.point1, c.point2);
    }
    Vector3 GetAverageOffset(ObjectInfo moveableInfo, ObjectInfo immoveableInfo)
    {
        Vector3 avrOffset = Vector3.zero;
        for (int i = 0; i < moveableInfo.connectPoints.Count; i++)
        {
            Vector3 offset = immoveableInfo.connectPoints[i].transform.position - moveableInfo.connectPoints[i].transform.position;
            avrOffset += offset;
        }
        avrOffset /= moveableInfo.connectPoints.Count;
        return avrOffset;
    }
    void UpdateConnectedPointsInfo(ObjectInfo moveableInfo, ObjectInfo immoveableInfo, SewPoint sp1, SewPoint sp2, Transform moveAbleTransform)
    {
        if (moveableInfo.stitchData != null)
            moveableInfo.stitchData.movedPositions.Add(moveAbleTransform.position);
        if (immoveableInfo.stitchData != null)
            immoveableInfo.stitchData.movedPositions.Add(moveAbleTransform.position);

        moveableInfo.IncementConnection();
        immoveableInfo.IncementConnection();

        sp1.IsConnected(true, 1, moveAbleTransform.position, moveableInfo.partType.ToString());
        sp2.IsConnected(true, 1, moveAbleTransform.position, immoveableInfo.partType.ToString());
    }

    void EyePartStitching(ObjectInfo moveableInfo, ObjectInfo immoveableInfo, SewPoint sp1, SewPoint sp2)
    {
       
        if (moveableInfo.stitchData.noOfConnections.Equals(moveableInfo.totalConnections))
        {
            moveableInfo.transform.DOMove(moveableInfo.movedPosition, 0.5f).SetEase(Ease.Linear).OnUpdate(() =>
            {
                UpdateConnections();
                var threadHandler = ServiceLocator.GetService<IThreadManager>();
                if (threadHandler != null)
                {
                    if (threadHandler.prevLine)
                        threadHandler.prevLine.SetPosition(0, threadHandler.detectedPoints[0].position);
                    if (threadHandler.instantiatedLine)
                        threadHandler.instantiatedLine.SetPosition(threadHandler.instantiatedLine.positionCount - 1, sp1.transform.position);
                }
            }).OnComplete(() =>
            {
                //Debug.LogError(" " + info1.transform.position + "" + info1.movedPosition);
                moveableInfo.transform.localPosition = moveableInfo.movedPosition;
                CheckIfLastConnectionUpdated(sp1, sp2, sp1.transform, sp2.transform, moveableInfo, immoveableInfo);
                moveableInfo.DOPause();
            });
        }
    }
    Tween tween1;
    public void ApplyForces(Transform p1, Transform p2)
    {

        if (p1 == null || p2 == null) return;

        if (p1.parent != null && p2.parent != null)
        {
            if (p1.parent == p2.parent)
            {
                return;
            }
        }

        if ((p1.parent == p2) || (p2.parent == p1)) return;

        var info1 = p1.GetComponent<ObjectInfo>();
        var info2 = p2.GetComponent<ObjectInfo>();

        if (info1 == null || info2 == null)
        {
            if (dynamicStitch)
            {
                info1 = p1.parent.parent.GetComponent<ObjectInfo>();
                info2 = p2.parent.parent.GetComponent<ObjectInfo>();
                if (p1.parent.parent.parent == p2.parent.parent.parent) return;

                info1.DisableWrongAlertText();
                info2.DisableWrongAlertText();
            }
            else
                return;
        }
  
        EndTweens();
        SewPoint sp1 = p1.GetComponent<SewPoint>();
        SewPoint sp2 = p2.GetComponent<SewPoint>();

        bool move1 = info1.moveable;
        bool move2 = info2.moveable;
        float tweenDuration = pullDuration;
        Sequence pullSeq = DOTween.Sequence();
        if(move1 && move2)
        {
            if (dynamicStitch)
            {
                int count = Mathf.Min(info1.connectPoints.Count, info2.connectPoints.Count);
                Vector3 avgMid = Vector3.zero;

                for (int i = 0; i < count; i++)
                {
                    avgMid += (info1.connectPoints[i].transform.position + info2.connectPoints[i].transform.position) * 0.5f;
                }
                avgMid /= count;

                pullSeq.Join(info1.transform.DOMove(Vector3.Lerp(info1.transform.position, avgMid, info1.pullForce), tweenDuration).SetEase(Ease.InOutSine));
                pullSeq.Join(info2.transform.DOMove(Vector3.Lerp(info2.transform.position, avgMid, info2.pullForce), tweenDuration).SetEase(Ease.InOutSine));
            }
           
        }
        else if(move1 && !move2)
        {
            if (dynamicStitch)
            {
                Vector3 avrOffset = Vector3.zero;
                //for (int i = 0; i < info1.connectPoints.Count; i++)
                //{
                //    Vector3 offset = info2.connectPoints[i].transform.position - info1.connectPoints[i].transform.position;
                //    avrOffset += offset;
                //}
                //avrOffset /= info1.connectPoints.Count;

                avrOffset = GetAverageOffset(info1, info2);

                Transform moveAbleTransform = null;
                if (info1.head)
                    moveAbleTransform = info1.transform.parent;
                else
                    moveAbleTransform = info1.transform;
                Vector3 targetPos = moveAbleTransform.position + avrOffset * info1.pullForce;
                if (info1.head)
                    targetPos.x = 0;
                UpdateConnectedPointsInfo(info1, info2, sp1, sp2, moveAbleTransform);

                //if (info1.stitchData != null)
                //    info1.stitchData.movedPositions.Add(moveAbleTransform.position);
                //if (info2.stitchData != null)
                //    info2.stitchData.movedPositions.Add(moveAbleTransform.position);

                //info1.IncementConnection();
                //info2.IncementConnection();

                //sp1.IsConnected(true, 1, moveAbleTransform.position, info1.partType.ToString());
                //sp2.IsConnected(true, 1, moveAbleTransform.position, info2.partType.ToString());
                //if (info1.stitchData.noOfConnections.Equals(info1.totalConnections) && info2.stitchData.noOfConnections.Equals(info2.totalConnections))
                //{
                //    LevelsHandler.instance.currentLevelMeta.noOfStitchedPart++;
                //}
                //if (info1.partType.Equals(PlushieActiveStitchPart.lefteye) || info1.partType.Equals(PlushieActiveStitchPart.righteye))
                //{
                //    if (info1.stitchData.noOfConnections.Equals(info1.totalConnections))
                //    {
                //        info1.transform.DOMove(info1.movedPosition, 0.5f).SetEase(Ease.Linear).OnUpdate(() =>
                //        {

                //            UpdateConnections();
                //            var threadHandler = ServiceLocator.GetService<IThreadManager>();
                //            if (threadHandler != null)
                //            {
                //                if (threadHandler.prevLine)
                //                    threadHandler.prevLine.SetPosition(0, threadHandler.detectedPoints[0].position);
                //                if (threadHandler.instantiatedLine)
                //                    threadHandler.instantiatedLine.SetPosition(threadHandler.instantiatedLine.positionCount - 1, sp1.transform.position);
                //            }
                //        }).OnComplete(() =>
                //        {
                //            //Debug.LogError(" " + info1.transform.position + "" + info1.movedPosition);
                //            info1.transform.localPosition = info1.movedPosition;
                //            SewPoint sp1 = p1.GetComponent<SewPoint>();
                //            SewPoint sp2 = p2.GetComponent<SewPoint>();
                //            //create eye connections
                //            //LevelsHandler.instance.currentLevelMeta.Connection(info1.connectPoints[0], info2.connectPoints[info2.connectPoints.Count - 1]);
                //            //LevelsHandler.instance.currentLevelMeta.Connection(info1.connectPoints[1], info2.connectPoints[info2.connectPoints.Count - 2]);
                //            CheckIfLastConnectionUpdated(sp1, sp2, p1, p2, info1, info2);

                //            info1.DOPause();
                //        });
                //    }

                //    return;
                //}
                if (info1.partType.Equals(PlushieActiveStitchPart.lefteye) || info1.partType.Equals(PlushieActiveStitchPart.righteye))
                {
                    EyePartStitching(info1, info2, sp1, sp2);
                    return;
                }
                //LevelsHandler.instance.currentLevelMeta.Connection(sp1, sp2);
                pullSeq.Join(moveAbleTransform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine));

                pullSeq.Join(
                    moveAbleTransform.DORotate(info1.originalRotation, tweenDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutSine)
                );
            }

        }
        else if(!move1 && move2)
        {
            if (dynamicStitch)
            {
                Vector3 avrOffset = Vector3.zero;
                //for (int i = 0; i < info2.connectPoints.Count; i++)
                //{
                //    Vector3 offset = info1.connectPoints[i].transform.position - info2.connectPoints[i].transform.position;
                //    avrOffset += offset;
                //}
                //avrOffset /= info2.connectPoints.Count;

                avrOffset = GetAverageOffset(info2, info1);

                Transform moveAbleTransform = null;
                if (info2.head)
                    moveAbleTransform = info2.transform.parent;
                else
                    moveAbleTransform = info2.transform;

                Vector3 targetPos = moveAbleTransform.position + avrOffset * info2.pullForce;
                if (info2.head)
                    targetPos.x = 0;
                UpdateConnectedPointsInfo(info2, info1, sp1, sp2, moveAbleTransform);

                //if (info1.stitchData != null)
                //    info1.stitchData.movedPositions.Add(moveAbleTransform.position);
                //if (info2.stitchData != null)
                //    info2.stitchData.movedPositions.Add(moveAbleTransform.position);

                //info1.IncementConnection();
                //info2.IncementConnection();

                //sp1.IsConnected(true, 1, moveAbleTransform.position, info1.partType.ToString());
                //sp2.IsConnected(true,1, moveAbleTransform.position, info2.partType.ToString());


                //if (info1.stitchData.noOfConnections.Equals(info1.totalConnections) && info2.stitchData.noOfConnections.Equals(info2.totalConnections))
                //{
                //    LevelsHandler.instance.currentLevelMeta.noOfStitchedPart++;
                //}

                //if (info2.partType.Equals(PlushieActiveStitchPart.lefteye) || info2.partType.Equals(PlushieActiveStitchPart.righteye))
                //{
                //    if (info2.stitchData.noOfConnections.Equals(info2.totalConnections))
                //    {
                //        info2.transform.DOMove(info2.movedPosition, 0.5f).SetEase(Ease.Linear).OnUpdate(() =>
                //        {
                //            UpdateConnections();
                //            var threadHandler = ServiceLocator.GetService<IThreadManager>();
                //            if (threadHandler != null)
                //            {
                //                if (threadHandler.prevLine)
                //                    threadHandler.prevLine.SetPosition(0, threadHandler.detectedPoints[0].position);
                //                if (threadHandler.instantiatedLine)
                //                    threadHandler.instantiatedLine.SetPosition(threadHandler.instantiatedLine.positionCount - 1, sp2.transform.position);
                //            }

                //        }).OnComplete(() =>
                //        {
                //            //Debug.LogError(" " + info2.transform.position + "" + info2.movedPosition);

                //            info2.transform.position = info2.movedPosition;

                //            SewPoint sp1 = p1.GetComponent<SewPoint>();
                //            SewPoint sp2 = p2.GetComponent<SewPoint>();
                //            //LevelsHandler.instance.currentLevelMeta.Connection(info1.connectPoints[0], info2.connectPoints[info2.connectPoints.Count - 1]);
                //            //LevelsHandler.instance.currentLevelMeta.Connection(info1.connectPoints[1], info2.connectPoints[info2.connectPoints.Count - 2]);

                //            CheckIfLastConnectionUpdated(sp1, sp2, p1, p2, info1, info2);
                //            //create eye connections

                //            info2.DOPause();
                //        });
                //    }

                //    return;
                //}
                
                if (info2.partType.Equals(PlushieActiveStitchPart.lefteye) || info2.partType.Equals(PlushieActiveStitchPart.righteye))
                {
                    EyePartStitching(info2, info1, sp1, sp2);
                    return;
                }
                pullSeq.Join(moveAbleTransform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine));
     
                pullSeq.Join(
                   moveAbleTransform.DORotate(info2.originalRotation, tweenDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutSine)
                );
            }

        }
      
      
        pullSeq.OnUpdate(() => {

            foreach (var connection in connections)
            {
             
                //if(connection.multipleLine)
                //    connection.UpdateLine(zVal, true);
                //else
                    connection.UpdateLine(zVal, false);

                float currentDist = Vector3.Distance(connection.point1.position, connection.point2.position);
                if (!connection.isLocked && currentDist < minDistance)
                {
                    connection.isLocked = true;

                }
            }
            foreach(var connection in LevelsHandler.instance.currentLevelMeta.cleanConnection)
            {
                connection.UpdateLine(zVal, false);
            }
        });
        pullSeq.OnComplete(() =>
        {
            if (info1 != null && info2 != null)
            {
                //LevelsHandler.instance.currentLevelMeta.Connection(sp1, sp2);
                CheckIfLastConnectionUpdated(sp1, sp2, p1, p2, info1, info2);

            }
        });
        tween1 = pullSeq;
    }
    void CheckIfLastConnectionUpdated(SewPoint sp1, SewPoint sp2, Transform p1, Transform p2, ObjectInfo info1, ObjectInfo info2)
    {
        if (sp1.attachmentId.Equals(sp2.attachmentId))
        {
            IncrementLinksPerPart(info1, info2);
        }
    }

    void IncrementLinksPerPart( ObjectInfo o1, ObjectInfo o2)
    {
        var threadHandler = ServiceLocator.GetService<IThreadManager>();

        if (o1.stitchData.noOfConnections.Equals(o1.totalConnections) && o2.stitchData.noOfConnections.Equals(o2.totalConnections))
        {
            o2.MarkStitched();
            o1.MarkStitched();

            points.Clear();
            var pointDetector = ServiceLocator.GetService<INeedleDetector>();
            if (pointDetector != null)
                pointDetector.pointsDetected.Clear();

            if (threadHandler != null)
            {
                threadHandler.pointIndex = 0;
                threadHandler.SetUndoValue(false);

            }
            //Invoke("UpdateProgress", 3);
        }
        //yield return new WaitForSeconds(0.0f);
        //StopCoroutine(IncrementLinksPerPart(s1, s2, o1, o2));
    }

    void EndTweens()
    {
        if (tween1 != null)
        {
            tween1.Kill();
            tween1 = null;
        }
       
    }
    public void UpdateConnections()
    {
        if (connections.Count == 0) return;
        for(int i = 0; i < connections.Count; i++)
        {
            connections[i].UpdateLine(zVal, false);
        }
    }
   
    public void DeleteAllThreadLinks()
    {
        if (connections.Count == 0) return;
        for (int i = 0; i < connections.Count; i++)
        {
            Destroy(connections[i].line.gameObject);
        }
        connections.Clear();
    }

    public Connections GetLastConnection()
    {
        if(connections.Count == 0) return null;
        return connections[connections.Count - 1];
    }
}
