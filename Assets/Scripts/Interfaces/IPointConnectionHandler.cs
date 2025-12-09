using System.Collections.Generic;
using UnityEngine;

public interface IPointConnectionHandler : IGameService
{
     List<SewPoint> points {  get; }
     List<Connections> connections { get; }
     float pullDuration {  get;}
     float maxPullDuration {  get; }
     float minPullDuration {  get; }
    float minDistance { get;}
    int minThreadStitchCount { get; }
    int maxThreadStitchCount { get; }
    int threadStitchCount { get; }
    bool dynamicStitch {  get; }
     Material originalMaterial {  get; }
     Material startToDetectMaterial {  get; }
     Material correctPointMaterial {  get; }
    Material wrongPointMaterial {  get; }
    List<SewPoint> wrongConnectPoint {  get; }
    LineRenderer linePrefab {  get; }
    void GetAttachedPointsToCreateLink(List<Transform> point);
    void CreateLinkBetweenPoints(SewPoint point1, SewPoint point2);
    void ManageConnetions(Connections c);
    void ApplyForces(Transform p1, Transform p2);
    void UpdateConnections();
    void DeleteAllThreadLinks();
    Connections GetLastConnection();
    void GetObjectInfoWrongAlertTextDisableOfPart(List<GameObject> list);
    void UpdateColorOfPoints();
    void ResetPointsList(List<SewPoint> list);
}
