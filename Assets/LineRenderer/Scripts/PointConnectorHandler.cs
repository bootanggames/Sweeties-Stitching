using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class PointConnectorHandler : MonoBehaviour, IPointConnectionHandler
{
    [field: SerializeField] public List<SewPoint> points { get; private set; }
    [field: SerializeField] public List<Connections> connections { get; private set; }
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
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] float zVal = -0.25f;
    [SerializeField] float rotationSpeed;
    [SerializeField] float pullForce;
    [SerializeField] bool moveStaticallyAtTheEndOfStitch;
    [field: SerializeField]public List<SewPoint> wrongConnectPoint { get; private set; }
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
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.RegisterEvent(GetAttachedPointsToCreateLink);
        GameEvents.PointConnectionHandlerEvents.onStopTweens.RegisterEvent(EndTweens);
        GameEvents.PointConnectionHandlerEvents.onUpdatingPullSpeed.RegisterEvent(SetPullSpeed);
        GameEvents.PointConnectionHandlerEvents.onUpdatingStitchCount.RegisterEvent(SetStitchCount);
        GameEvents.PointConnectionHandlerEvents.onSettingPlushieLevel2.RegisterEvent(ActivateDynamicStitch);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPointConnectionHandler>(this);
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.UnregisterEvent(GetAttachedPointsToCreateLink);
        GameEvents.PointConnectionHandlerEvents.onStopTweens.UnregisterEvent(EndTweens);
        GameEvents.PointConnectionHandlerEvents.onUpdatingPullSpeed.UnregisterEvent(SetPullSpeed);
        GameEvents.PointConnectionHandlerEvents.onUpdatingStitchCount.UnregisterEvent(SetStitchCount);
        GameEvents.PointConnectionHandlerEvents.onSettingPlushieLevel2.UnregisterEvent(ActivateDynamicStitch);
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
   
    public void GetAttachedPointsToCreateLink(List<Transform> point)
    {
        if (point.Count <= 1) return;
        foreach (Transform t in point)
        {
            if (!points.Contains(t.GetComponent<SewPoint>()))
                points.Add(t.GetComponent<SewPoint>());
        }
        SewPoint sp1 = points[points.Count - 2];
        SewPoint sp2 = points[points.Count - 1];
        ObjectInfo o_info1 = sp1.transform.parent.parent.GetComponent<ObjectInfo>();
        ObjectInfo o_info2 = sp2.transform.parent.parent.GetComponent<ObjectInfo>();
        SewPoint s_FirstOne = points[0];
        if (s_FirstOne.startFlag)
        {
            if (points.Count > 0 && points.Count % 2 != 0)
            {
                if (!sp1.nextConnectedPointId.Equals(sp2.attachmentId))
                {
                    if(!wrongConnectPoint.Contains(sp2))
                        wrongConnectPoint.Add( sp2);
                    sp1.pointMesh.material = wrongPointMaterial;
                    sp2.pointMesh.material = wrongPointMaterial;
                }
            }
            else
            {
                if(wrongConnectPoint == null && wrongConnectPoint.Count == 0)
                    CheckIfLastConnectionUpdated(sp1, sp2, points[points.Count - 2].transform, points[points.Count - 1].transform, o_info1, o_info2);
            }
        }

        CreateLinkBetweenPoints(sp1, sp2);
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

            //Debug.LogError("existingConnection " + existingConnection.point1.name + " " + existingConnection.point2.name);
            //return;
        }
        if (dynamicStitch)
            NewConnection(point1.transform, point2.transform, true, false, threadStitchCount);
        else
        {
            if (threadStitchCount == 1)
                NewConnection(point1.transform, point2.transform, true, false, threadStitchCount);
            else
                NewConnection(point1.transform, point2.transform, true, true, threadStitchCount);
        }
    }

    void NewConnection(Transform p1, Transform p2, bool applyPullForce, bool multiple, int stitchCount)
    {
        if (connections.Exists(c =>
        (c.point1 == p1 && c.point2 == p2) ||
        (c.point1 == p2 && c.point2 == p1)))
            return;
        Connections connection = new Connections(p1, p2, linePrefab, zVal, multiple, stitchCount);
        connections.Add(connection);

      
        SewPoint s_FirstOne = points[0];

        SewPoint s1 = p1.GetComponent<SewPoint>();
        SewPoint s2 = p2.GetComponent<SewPoint>();
        if (!s_FirstOne.startFlag)
        {
            s1.pointMesh.material = wrongPointMaterial;
            s2.pointMesh.material = wrongPointMaterial;
         
            return;
        }
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
                if (p1.parent.parent.parent == p2.parent.parent.parent)
                {
                    return;
                }
            }
            else
                return;
        }
        EndTweens();

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
            else
            {
                Vector3 midPoint = (p1.position + p2.position) / 2;
                pullSeq.Join(p1.DOMove(midPoint, tweenDuration).SetEase(Ease.InOutSine));
                pullSeq.Join(p1.DORotate(info1.originalRotation, tweenDuration));

                pullSeq.Join(p2.DOMove(midPoint, tweenDuration).SetEase(Ease.InOutSine));
                pullSeq.Join(p2.DORotate(info2.originalRotation, tweenDuration));

            }
          
        }
        else if(move1 && !move2)
        {
            if (dynamicStitch)
            {
                Vector3 avrOffset = Vector3.zero;
                for (int i = 0; i < info1.connectPoints.Count; i++)
                {
                    Vector3 offset = info2.connectPoints[i].transform.position - info1.connectPoints[i].transform.position;
                    avrOffset += offset;
                }
                avrOffset /= info1.connectPoints.Count;

                Transform moveAbleTransform = null;
                if (info1.head)
                    moveAbleTransform = info1.transform.parent;
                else
                    moveAbleTransform = info1.transform;

                info1.movedPositions.Add(moveAbleTransform.position);
                info2.movedPositions.Add(moveAbleTransform.position);
                Vector3 targetPos = moveAbleTransform.position + avrOffset * info1.pullForce;

                //if (info1.partType.Equals(PlushieActiveStitchPart.lefteye) || info1.partType.Equals(PlushieActiveStitchPart.righteye))
                //{
                //    if (info1.noOfConnections.Equals(info1.totalConnections))
                //    {
                //        info1.transform.DOMove(info1.movedPosition, 0.5f).SetEase(Ease.Linear).OnUpdate(() =>
                //        {
                //            UpdateConnections();
                //        }).OnComplete(() =>
                //        {
                //            SewPoint sp1 = p1.GetComponent<SewPoint>();
                //            SewPoint sp2 = p2.GetComponent<SewPoint>();
                //            CheckIfLastConnectionUpdated(sp1, sp2, p1, p2, info1, info2);
                //            info1.DOPause();
                //        });
                //    }

                //    return;
                //}
                pullSeq.Join(moveAbleTransform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine));
               
                pullSeq.Join(
                    moveAbleTransform.DORotate(info1.originalRotation, tweenDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutSine)
                );
            }
            else
            {
                pullSeq.Join(p1.DOMove(p2.transform.position, tweenDuration).SetEase(Ease.InOutSine));
                pullSeq.Join(p1.DORotate(info1.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
            }

        }
        else if(!move1 && move2)
        {
            if (dynamicStitch)
            {
                Vector3 avrOffset = Vector3.zero;
                for (int i = 0; i < info2.connectPoints.Count; i++)
                {
                    Vector3 offset = info1.connectPoints[i].transform.position - info2.connectPoints[i].transform.position;
                    avrOffset += offset;
                }
                avrOffset /= info2.connectPoints.Count;
                Transform moveAbleTransform = null;
                if (info2.head)
                    moveAbleTransform = info2.transform.parent;
                else
                    moveAbleTransform = info2.transform;
                info1.movedPositions.Add(moveAbleTransform.position);
                info2.movedPositions.Add(moveAbleTransform.position);
                Vector3 targetPos = moveAbleTransform.position + avrOffset * info2.pullForce;

                //if (info2.partType.Equals(PlushieActiveStitchPart.lefteye) || info2.partType.Equals(PlushieActiveStitchPart.righteye))
                //{
                //    if (info2.noOfConnections.Equals(info2.totalConnections))
                //    {
                //        info2.transform.DOMove(info2.movedPosition, 0.5f).SetEase(Ease.Linear).OnUpdate(() =>
                //        {
                //            UpdateConnections();
                //        }).OnComplete(() =>
                //        {
                //            SewPoint sp1 = p1.GetComponent<SewPoint>();
                //            SewPoint sp2 = p2.GetComponent<SewPoint>();
                //            CheckIfLastConnectionUpdated(sp1, sp2, p1, p2, info1, info2);
                //            info2.DOPause();
                //        });
                //    }

                //    return;
                //}
                pullSeq.Join(moveAbleTransform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine));
     
                pullSeq.Join(
                   moveAbleTransform.DORotate(info2.originalRotation, tweenDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutSine)
                );
            }
            else
            {
                pullSeq.Join(p2.DOMove(p1.transform.position, tweenDuration).SetEase(Ease.InOutSine));
                pullSeq.Join(p2.DORotate(info2.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
            }

        }
        else
        {
            
        }
      
        pullSeq.OnUpdate(() => {

            foreach (var connection in connections)
            {
             
                if(connection.multipleLine)
                    connection.UpdateLine(zVal, true);
                else
                    connection.UpdateLine(zVal, false);

                float currentDist = Vector3.Distance(connection.point1.position, connection.point2.position);
                if (!connection.isLocked && currentDist < minDistance)
                {
                    connection.isLocked = true;

                }
            }
        });
        pullSeq.OnComplete(() =>
        {
            SewPoint sp1 = p1.GetComponent<SewPoint>();
            SewPoint sp2 = p2.GetComponent<SewPoint>();
            CheckIfLastConnectionUpdated(sp1, sp2, p1, p2, info1, info2);
        });
        tween1 = pullSeq;
    }
    void CheckIfLastConnectionUpdated(SewPoint sp1, SewPoint sp2, Transform p1, Transform p2, ObjectInfo info1, ObjectInfo info2)
    {

        if (p1.parent == p2.parent)
            return;
        if (sp1.connected && sp2.connected) return;
        if (dynamicStitch)
        {
            if (p1.parent.parent.parent == p2.parent.parent.parent) return;
        }

        if (sp1.attachmentId.Equals(sp2.attachmentId))
            StartCoroutine(IncrementLinksPerPart(sp1, sp2, info1, info2));
    }
    float completeTime = 0;
    IEnumerator IncrementLinksPerPart(SewPoint s1, SewPoint s2 , ObjectInfo o1, ObjectInfo o2)
    {
       var threadHandler = ServiceLocator.GetService<IThreadManager>();

        s1.connected = true;
        s2.connected = true;
        s1.pointMesh.material = correctPointMaterial;
        s2.pointMesh.material = correctPointMaterial;
        s1.name = s1.attachmentId.ToString();
        s2.name = s2.attachmentId.ToString();
        o1.noOfConnections++;
        o2.noOfConnections++;
        if (threadHandler != null)
            threadHandler.ScaleDownAllPoints();
        yield return new WaitForSeconds(0.5f);
        if (o1.noOfConnections.Equals(o1.totalConnections) && o2.noOfConnections.Equals(o2.totalConnections))
        {
           
            o2.moveable = false;
            o1.moveable = false;
            o2.MarkStitched();
            o1.MarkStitched();
           
            points.Clear();
            var pointDetector = ServiceLocator.GetService<INeedleDetector>();
            if (pointDetector != null)
                pointDetector.pointsDetected.Clear();

            if (threadHandler != null)
                threadHandler.pointIndex = 0;

            //Invoke("UpdateProgress", 3);
        }
        StopCoroutine(IncrementLinksPerPart(s1, s2, o1, o2));

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
