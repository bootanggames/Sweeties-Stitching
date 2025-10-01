using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PointConnectorHandler : MonoBehaviour
{
    List<SewPoint> points = new List<SewPoint>();
    List<Connections> connections = new List<Connections>();

    [SerializeField] float pullDuration = 1.5f; 
    [SerializeField] float minDistance = 0.05f;
    void GetAttachedPointsToCreateLink(SewPoint point)
    {
        points.Add(point);
        if (points.Count % 2 != 0)
            return;

        CreateLinkBetweenPoints(points[points.Count - 2], points[points.Count - 1]);
    }

    void CreateLinkBetweenPoints(SewPoint point1, SewPoint point2)
    {
        Connections connection = new Connections(point1.transform, point2.transform);
        connections.Add(connection);

        PullConnectors();
    }

    void PullConnectors()
    {
        foreach(Connections c in connections)
        {
            ManageConnetions(c);
        }
    }

    void ManageConnetions(Connections c)
    {
        ApplyPullForce(c.point1, c.point2);
    }

    void ApplyPullForce(Transform p1, Transform p2)
    {
        if (p1 == null || p2 == null) return;
        if (p1.parent == p2.parent) return;

        Vector3 midpoint = (p1.position + p2.position) / 2f;

        float dist = Vector3.Distance(p1.position, p2.position);
        if (dist <= minDistance) return;

        p1.DOMove(midpoint, pullDuration).SetEase(Ease.InOutSine);
        p2.DOMove(midpoint, pullDuration).SetEase(Ease.InOutSine);
    }
}
