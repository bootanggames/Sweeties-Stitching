using DG.Tweening;
using System.Collections.Generic;
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

    [SerializeField] float tolerance = 0.05f;
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] float zVal = -0.25f;
    [SerializeField] float rotationSpeed;
    [SerializeField] float pullForce;

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

        if (point.Count % 2 != 0)
        {
            NewConnection(point[point.Count - 2].transform, point[point.Count - 1].transform, false, false, 0);
            var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
            if (needleDetecto != null)
            {
                needleDetecto.detect = true;
            }
            return;
        }
        foreach (Transform t in point)
        {
            if (!points.Contains(t.GetComponent<SewPoint>()))
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
        if (applyPullForce)
            ManageConnetions(connection);
        else
            connection.isLocked = true;

        var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
        if(needleDetecto != null)
        {
            needleDetecto.detect = false;
        }
    }

    void IncrementConnections(Level_Metadata currentLevel)
    {
        currentLevel.noOfCorrectLinks++;
        if (currentLevel.noOfCorrectLinks == currentLevel.totalCorrectLinks)
        {
            GameEvents.GameCompleteEvents.onGameWin.RaiseEvent();
            Invoke("CallGameWinPanel", 1.5f);
        }
    }
    void CallGameWinPanel()
    {
        GameEvents.GameCompleteEvents.onGameComplete.RaiseEvent();
        CancelInvoke("CallGameWinPanel");
    }
    public void ManageConnetions(Connections c)
    {
        ApplyForces(c.point1, c.point2);
    }
    Tween tween1;
    public void ApplyForces(Transform p1, Transform p2)
    {
        if (p1 == null || p2 == null) return;
        if (p1.parent != null && p2.parent != null)
        {
            if (p1.parent == p2.parent) return;
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
            }
            else
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
            if (dynamicStitch)
            {
                int count = Mathf.Min(info1.connectPoints.Count, info2.connectPoints.Count);
                Vector3 avgMid = Vector3.zero;

                for (int i = 0; i < count; i++)
                {
                    avgMid += (info1.connectPoints[i].transform.position + info2.connectPoints[i].transform.position) * 0.5f;
                }
                avgMid /= count;

                pullSeq.Join(info1.transform.DOMove(Vector3.Lerp(info1.transform.position, avgMid, pullForce), tweenDuration).SetEase(Ease.InOutSine));
                pullSeq.Join(info2.transform.DOMove(Vector3.Lerp(info2.transform.position, avgMid, pullForce), tweenDuration).SetEase(Ease.InOutSine));
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

                Vector3 targetPos = moveAbleTransform.position + avrOffset * pullForce;

              
                pullSeq.Join(moveAbleTransform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine));
                Vector3 lookDir = moveAbleTransform.position - info1.transform.position;

                Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);

                Vector3 euler = targetRot.eulerAngles;
                euler.x = 0f;
                euler.y = 0f;

                pullSeq.Join(
                    moveAbleTransform.DORotate(euler, tweenDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutSine)
                );
            }
            else
            {
                pullSeq.Join(p1.DOMove(p2.transform.position, tweenDuration).SetEase(Ease.InOutSine));
                pullSeq.Join(p1.DORotate(info1.originalRotation, tweenDuration).SetEase(Ease.InOutSine));
            }

            //Debug.LogError(" apply force "+ midPointParent1);
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

                Vector3 targetPos = moveAbleTransform.position + avrOffset * pullForce;

                pullSeq.Join(moveAbleTransform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine));
                Vector3 lookDir = info1.transform.position - moveAbleTransform.position;

                Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);

                Vector3 euler = targetRot.eulerAngles;
                euler.x = 0f;
                euler.y = 0f;

                pullSeq.Join(
                   moveAbleTransform.DORotate(euler, tweenDuration, RotateMode.Fast)
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
        pullSeq.OnComplete(() =>
        {
            var needleDetecto = ServiceLocator.GetService<INeedleDetector>();
            if (needleDetecto != null)
            {
                needleDetecto.detect = true;
            }
            Level_Metadata currentLevel = LevelsHandler.instance.levels[LevelsHandler.instance.levelIndex].GetComponent<Level_Metadata>();

            if (currentLevel)
            {
                SewPoint sp1 = p1.GetComponent<SewPoint>();
                SewPoint sp2 = p2.GetComponent<SewPoint>();

                if (p1.parent == p2.parent) return;
                if (sp1.connected || sp2.connected) return;
                if (dynamicStitch)
                {
                    if (p1.parent.parent.parent == p2.parent.parent.parent) return;
                }

                if (sp1.attachmentId.Equals(sp2.attachmentId))
                {
                    sp1.connected = true;
                    sp2.connected = true;
                    IncrementConnections(currentLevel);
                }
                else
                {
                    if (sp1.alternativeAttachments.Length > 0)
                    {
                        if (sp1.alternativeAttachments.Contains(sp2.attachmentId))
                        {
                            sp1.connected = true;
                            sp2.connected = true;
                            IncrementConnections(currentLevel);
                        }
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
