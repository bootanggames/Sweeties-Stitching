using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointConnectorHandler : MonoBehaviour, IPointConnectionHandler
{
    [field: SerializeField] public List<SewPoint> points {  get; private set; }
    [field: SerializeField]public List<Connections> connections {  get; private set; }
    [field: SerializeField] public float pullDuration {  get; private set; }
    [field: SerializeField] public float minDistance {  get; private set; }

    [field: SerializeField] public float maxPullDuration {  get; private set; }

    [SerializeField]float tolerance = 0.05f;
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] float zVal = -0.25f;
    [SerializeField] float rotationSpeed;
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
        GameEvents.PointConnectionHandlerEvents.onGettingMaxSpeed.RegisterEvent(SetMaxPullSpeed);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPointConnectionHandler>(this);
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.UnregisterEvent(GetAttachedPointsToCreateLink);
        GameEvents.PointConnectionHandlerEvents.onStopTweens.UnregisterEvent(EndTweens);
        GameEvents.PointConnectionHandlerEvents.onUpdatingPullSpeed.UnregisterEvent(SetPullSpeed);
        GameEvents.PointConnectionHandlerEvents.onGettingMaxSpeed.UnregisterEvent(SetMaxPullSpeed);

    }
    float SetMaxPullSpeed()
    {
        return maxPullDuration;
    }
    void SetPullSpeed(float speed)
    {
        pullDuration = speed;
    }
    public void GetAttachedPointsToCreateLink(List<Transform> point)
    {
        if (point.Count <= 1) return;

        if (point.Count % 2 != 0)
        {
            NewConnection(point[point.Count - 2].transform, point[point.Count - 1].transform, false, false);
            return;
        }
        foreach (Transform t in point)
        {
            if(!points.Contains(t.GetComponent<SewPoint>()))
                points.Add(t.GetComponent<SewPoint>());
        }
        CreateLinkBetweenPoints(points[points.Count - 2], points[points.Count - 1]);
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
            return;
        }

        NewConnection(point1.transform, point2.transform, true, false);
    }

    void NewConnection(Transform p1, Transform p2, bool applyPullForce, bool multiple)
    {
        if (connections.Exists(c =>
        (c.point1 == p1 && c.point2 == p2) ||
        (c.point1 == p2 && c.point2 == p1)))
            return;

        Connections connection = new Connections(p1, p2, linePrefab, zVal, multiple);
        connections.Add(connection);
        if (applyPullForce)
            ManageConnetions(connection);
        else
            connection.isLocked = true;
    }
    public void ManageConnetions(Connections c)
    {
        ApplyForces(c.point1, c.point2);
    }
    Tween tween1;
   
    public void ApplyForces(Transform p1, Transform p2)
    {
        if (p1 == null || p2 == null) return;
        if (p1.parent == p2.parent) return;

        var info1 = p1.GetComponent<ObjectInfo>();
        var info2 = p2.GetComponent<ObjectInfo>();
        if (info1 == null || info2 == null)
        {
            //info1 = p1.parent.parent.GetComponent<ObjectInfo>();
            //info2 = p2.parent.parent.GetComponent<ObjectInfo>();
            return;
        }
        EndTweens();
        bool move1 = info1.moveable;
        bool move2 = info2.moveable;
        float dist = Vector3.Distance(p1.position, p2.position);
        float tweenDuration = pullDuration;
        Sequence pullSeq = DOTween.Sequence();
        if(move1 && move2)
        {
            Vector3 midPoint = (p1.position + p2.position) / 2;
            pullSeq.Join(p1.DOMove(midPoint, tweenDuration).SetEase(Ease.InOutSine));
            pullSeq.Join(p1.DORotate(info1.originalRotation, tweenDuration));

            pullSeq.Join(p2.DOMove(midPoint, tweenDuration).SetEase(Ease.InOutSine));
            pullSeq.Join(p2.DORotate(info2.originalRotation, tweenDuration));
        }
        else if(move1 && !move2)
        {
            //Transform topParent = p1.parent.parent;
            //Vector3 midPointForParent1 = Vector3.zero;

            //foreach (SewPoint s in info1.connectPoints)
            //    midPointForParent1 += s.transform.position;

            //Vector3 midPointParent1 = midPointForParent1 / info1.connectPoints.Count;

            //Vector3 dCubeAttach = (p1.position - topParent.position).normalized;
            //Vector3 dAttach0Attach1U = (midPointParent1 - p1.position);
            //Vector3 dAttach0Attach1 = dAttach0Attach1U.normalized;

            //pullSeq.Join(p1.parent.DOMove(midPointParent1, tweenDuration).SetEase(Ease.InOutSine));

            //float angleFactor = Vector3.Angle(dCubeAttach, dAttach0Attach1) / 180;
            //float rotationSpeed = angleFactor * -Mathf.Sign(Vector3.Dot(dCubeAttach, dAttach0Attach1));

            //pullSeq.Join(p1.parent.DORotate(Vector3.forward, tweenDuration).SetEase(Ease.InOutSine));

            pullSeq.Join(p1.DOMove(p2.transform.position, tweenDuration).SetEase(Ease.InOutSine));
            pullSeq.Join(p1.DORotate(info1.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
            //Debug.LogError(" apply force "+ midPointParent1);
        }
        else if(!move1 && move2)
        {
            pullSeq.Join(p2.DOMove(p1.transform.position, tweenDuration).SetEase(Ease.InOutSine));
            pullSeq.Join(p2.DORotate(info2.originalRotation, tweenDuration).SetEase(Ease.InOutSine));

        }
        else
        {

        }
      
        pullSeq.OnUpdate(() => {

            foreach (var connection in connections)
            {
                //if (!IsRelatedConnection(connection, p1, p2))
                //    continue;
                if(connection.multipleLine)
                    connection.UpdateLine(zVal, true);
                else
                    connection.UpdateLine(zVal, false);

                float currentDist = Vector3.Distance(connection.point1.position, connection.point2.position);

                if (!connection.isLocked && currentDist < minDistance)
                {
                    connection.isLocked = true;
                    if (info1.shouldBeChild)
                    {
                        info1.transform.SetParent(info2.transform);
                        info1.transform.localEulerAngles = Vector3.zero;
                    }
                    else if (info2.shouldBeChild)
                    {
                        info2.transform.SetParent(info1.transform);
                        info2.transform.localEulerAngles = Vector3.zero;

                    }

                }
            }
        });
        tween1 = pullSeq;
    }
    bool IsRelatedConnection(Connections conn, Transform a, Transform b)
    {

        if (conn.point1 == a || conn.point2 == a || conn.point1 == b || conn.point2 == b)
            return true;

        Transform aRoot = a.parent != null ? a.parent : a;
        Transform bRoot = b.parent != null ? b.parent : b;
        Transform c1Root = conn.point1.parent != null ? conn.point1.parent : conn.point1;
        Transform c2Root = conn.point2.parent != null ? conn.point2.parent : conn.point2;

        return c1Root == aRoot || c1Root == bRoot || c2Root == aRoot || c2Root == bRoot;
    }

    void EndTweens()
    {
        if (tween1 != null /*&& tween1.IsActive()*/)
        {
            tween1.Kill();
            tween1 = null;
        }
       
    }
   
}
