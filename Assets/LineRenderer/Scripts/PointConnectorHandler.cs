using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointConnectorHandler : MonoBehaviour, IPointConnectionHandler
{
    public List<SewPoint> points {  get; private set; }
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
        if (point.Count < 2)
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
    }


    public void ManageConnetions(Connections c)
    {
        ApplyPullForce(c.point1, c.point2);
    }
    Tween tween1;
    Tween tween2;
    public void ApplyPullForce(Transform p1, Transform p2)
    {
        if (p1 == null || p2 == null) return;
        if (p1.parent == null || p2.parent == null) return;
        if (p1.parent == p2.parent) return;

        EndTweens();

        Transform parent1 = p1.parent;
        Transform parent2 = p2.parent;

        Vector3 childPos1 = p1.position;
        Vector3 childPos2 = p2.position;

        Vector3 parentMid = (parent1.position + parent2.position) * 0.5f;

        float dist = Vector3.Distance(childPos1, childPos2);
        float tweenDuration = pullDuration;

        if (dist <= (minDistance + tolerance))
        {
            Vector3 dir = (childPos1 - childPos2).normalized;
            Vector3 midpoint = (childPos1 + childPos2) * 0.5f;

            Vector3 desiredChild1 = midpoint + dir * (minDistance * 0.5f);
            Vector3 desiredChild2 = midpoint - dir * (minDistance * 0.5f);

            float equalY = (desiredChild1.y + desiredChild2.y) * 0.5f;
            desiredChild1.y = equalY;
            desiredChild2.y = equalY;

            Vector3 targetParent1 = parent1.position + (desiredChild1 - childPos1);
            Vector3 targetParent2 = parent2.position + (desiredChild2 - childPos2);

            parent1.position = targetParent1;
            parent2.position = targetParent2;

            Sequence restoreSeq = DOTween.Sequence();
            var info1 = parent1.GetComponent<ObjectInfo>();
            var info2 = parent2.GetComponent<ObjectInfo>();
            if (info1) restoreSeq.Join(parent1.DORotate(info1.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
            if (info2) restoreSeq.Join(parent2.DORotate(info2.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
            tween1 = restoreSeq;
            return;
        }

        Vector3 dirPull = (childPos1 - childPos2).normalized;
        Vector3 desiredChildPos1 = parentMid + dirPull * (minDistance * 0.5f);
        Vector3 desiredChildPos2 = parentMid - dirPull * (minDistance * 0.5f);

        float equalPullY = (desiredChildPos1.y + desiredChildPos2.y) * 0.5f;
        desiredChildPos1.y = equalPullY;
        desiredChildPos2.y = equalPullY;

        Vector3 targetParentPos1 = parent1.position + (desiredChildPos1 - childPos1);
        Vector3 targetParentPos2 = parent2.position + (desiredChildPos2 - childPos2);

        Sequence pullSeq = DOTween.Sequence();
        pullSeq.Join(parent1.DOMove(targetParentPos1, tweenDuration).SetEase(Ease.InOutSine));
        pullSeq.Join(parent2.DOMove(targetParentPos2, tweenDuration).SetEase(Ease.InOutSine));
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
}
