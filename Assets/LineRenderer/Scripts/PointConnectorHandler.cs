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
    [SerializeField]float tolerance = 0.05f;
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] float zVal = -0.25f;
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
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPointConnectionHandler>(this);
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.UnregisterEvent(GetAttachedPointsToCreateLink);
        GameEvents.PointConnectionHandlerEvents.onStopTweens.UnregisterEvent(EndTweens);
    }
    public void GetAttachedPointsToCreateLink(List<Transform> point)
    {
        if (point.Count == 0) return;

        if (point.Count % 2 != 0)
            return;
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

        Connections connection = new Connections(point1.transform, point2.transform, linePrefab, zVal);

        Connections existing = null;

        if (!connections.Contains(connection))
            connections.Add(connection);

        if (existing == null)
            ManageConnetions(connection);
        else
        {
            if (!existing.isLocked)
                ManageConnetions(existing); 
        }

        //ApplyForceToMultipleParentObjects(connections);
    }

    public void ManageConnetions(Connections c)
    {
        ApplyForces(c.point1, c.point2);
    }
    Tween tween1;
    Tween tween2;

    //parent1--body
    //parent2--arm
    //points are child
    //get parent reference only to check if moveable
    //attract point 1 to point 2 not parent1 to parent2
    public void ApplyForces(Transform p1, Transform p2)
    {
        if (p1 == null || p2 == null) return;
        //if (p1.parent == null || p2.parent == null) return;
        //if (p1.parent == p2.parent) return;
        EndTweens();

        //Transform parent1 = p1.parent;
        //Transform parent2 = p2.parent;
        var info1 = p1.GetComponent<ObjectInfo>();
        var info2 = p2.GetComponent<ObjectInfo>();

        float dist = Vector3.Distance(p1.position, p2.position);
        float tweenDuration = pullDuration;
        Sequence pullSeq = DOTween.Sequence();
        Debug.LogError(" " + info1.moveable);
        if (info1.moveable)
        {
            Debug.LogError(" " + p2.transform.position);

            pullSeq.Join(p1.DOMove(p2.transform.position, tweenDuration).SetEase(Ease.InOutSine));
            pullSeq.Join(p1.DORotate(info1.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
        }
        if (info2.moveable)
        {
            pullSeq.Join(p2.DOMove(p1.transform.position, tweenDuration).SetEase(Ease.InOutSine));
            pullSeq.Join(p2.DORotate(info2.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
        }

        pullSeq.OnUpdate(() => {
            foreach (var connection in connections)
            {
                if (connection.point1 == p1 || connection.point2 == p2 ||
                    connection.point1 == p1 || connection.point2 == p2)
                {
                    connection.UpdateLine(zVal);
                }
            }
            Debug.LogError(" " + dist);

            if (dist < minDistance)
            {
                foreach (var connection in connections)
                {
                    if (connection.point1 == p1 || connection.point2 == p2 ||
                     connection.point1 == p1 || connection.point2 == p2)
                    {
                        connection.isLocked = true;
                        break;
                    }
                }
            }
        });
        tween1 = pullSeq;
    }
    
    void EndTweens()
    {
        if (tween1 != null && tween1.IsActive())
        {
            tween1.Kill();
            tween1 = null;
        }

        if (tween2 != null && tween2.IsActive())
        {
            tween2.Kill();
            tween2 = null;
        }
    }
   

    public void ApplyForceToMultipleParentObjects(List<Connections> _connectPoints)
    {
        foreach (Connections c in _connectPoints)
        {
            //ApplyPullForce(c.point1, c.point2);
        }
    }
}
