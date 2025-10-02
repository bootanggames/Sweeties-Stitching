using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PointConnectorHandler : MonoBehaviour, IPointConnectionHandler
{
    public List<SewPoint> points {  get; private set; }
    public List<Connections> connections {  get; private set; }
    [field: SerializeField] public float pullDuration {  get; private set; }
    [field: SerializeField] public float minDistance {  get; private set; }

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
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPointConnectionHandler>(this);
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.UnregisterEvent(GetAttachedPointsToCreateLink);
    }
    public void GetAttachedPointsToCreateLink(SewPoint point, LineRenderer _line)
    {
        points.Add(point);
        if (points.Count % 2 != 0)
            return;

        CreateLinkBetweenPoints(points[points.Count - 2], points[points.Count - 1], _line);
    }

    public void CreateLinkBetweenPoints(SewPoint point1, SewPoint point2, LineRenderer _line)
    {
        Connections connection = new Connections(point1.transform, point2.transform);
        connections.Add(connection);

        PullConnectors( _line);
    }


    public void PullConnectors(LineRenderer _line)
    {
        foreach(Connections c in connections)
        {
            ManageConnetions(c, _line);
        }
    }

    public void ManageConnetions(Connections c, LineRenderer _line)
    {
        ApplyPullForce(c.point1, c.point2, _line);
    }
    Tween tween1;
    Tween tween2;
    public void ApplyPullForce(Transform p1, Transform p2, LineRenderer _line)
    {
        if (_line == null) return;
        if (p1 == null || p2 == null) return;
        if (p1.parent == p2.parent) return;

        Vector3 midpoint = (p1.position + p2.position) / 2f;
        float dist = Vector3.Distance(p1.position, p2.position);
        if (tween1 != null && tween1.IsActive())
            tween1.Kill();

        if (tween2 != null && tween2.IsActive())
            tween2.Kill();
        if (dist <= minDistance) return;

        Transform parent1 = p1.parent;
        Transform parent2 = p2.parent;

        Vector3 dir1 = (midpoint - parent1.position).normalized;
        Vector3 dir2 = (midpoint - parent2.position).normalized;

        float moveAmount = dist / 2f;
        float tweenDuration = pullDuration;

        Vector3 targetPos1 = parent1.position + dir1 * moveAmount;
        Vector3 targetPos2 = parent2.position + dir2 * moveAmount;

        float equalY = (targetPos1.y + targetPos2.y) / 2f; 
        targetPos1.y = equalY;
        targetPos2.y = equalY;

      
      
        Sequence seq = DOTween.Sequence();
        seq.Join(parent1.DOMove(targetPos1, tweenDuration).SetEase(Ease.InOutSine));
        ObjectInfo parentInfo = parent1.GetComponent<ObjectInfo>();
        seq.Join(parent1.DORotate(parentInfo.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
        tween1 = seq;


        Sequence seq2 = DOTween.Sequence();
        seq2.Join(parent2.DOMove(targetPos2, tweenDuration).SetEase(Ease.InOutSine));
        ObjectInfo parent2Info = parent2.GetComponent<ObjectInfo>();

        seq2.Join(parent2.DORotate(parent2Info.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
        tween2 = seq2;

        DOTween.To(() => 0f, _ =>
        {
            if (_line != null)
            {
                _line.SetPosition(0, p1.position);
                _line.SetPosition(1, p2.position);
            }
        }, 1f, tweenDuration).SetEase(Ease.Linear);
    }
}
