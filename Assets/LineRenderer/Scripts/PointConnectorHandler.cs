using System.Collections.Generic;
using UnityEngine;

public class PointConnectorHandler : MonoBehaviour
{
    List<SewPoint> points = new List<SewPoint>();
    void GetAttachedPointsToCreateLink(SewPoint point)
    {
        points.Add(point);
        if (points.Count % 2 != 0)
            return;

        CreateLinkBetweenPoints(points[points.Count - 2], points[points.Count - 1]);
    }

    void CreateLinkBetweenPoints(SewPoint point1, SewPoint point2)
    {

    }
}
