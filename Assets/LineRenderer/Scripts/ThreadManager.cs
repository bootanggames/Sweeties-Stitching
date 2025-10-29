using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadManager : MonoBehaviour, IThreadManager
{
    [field: SerializeField] public bool threadInput { get; private set; }
    [field: SerializeField] public bool freeForm { get; private set; }
    [field: SerializeField] public List<Transform> detectedPoints {  get; private set; }
    //[field: SerializeField] public List<Color> threadColor { get; private set; }

    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] Vector3 currentRopeStartPosition;
    public Vector3 prevMouseDragPosition { get; private set; }

    [SerializeField] float minDistanceBetweenSewPoints;
    [SerializeField] int threadMaxLength;

    [SerializeField] private Transform threadParent;
    [SerializeField] int detectedPointsCount = 0;

    [field: SerializeField] public LineRenderer instantiatedLine {  get; private set; }
    [SerializeField] Transform pointsLinkParent;
    public Transform lastConnectedPoint { get; private set; }
    [SerializeField] float zVal;
    [SerializeField] Transform startPoint;
    [SerializeField] Vector3 direction;
    [field: SerializeField] public int threadIndex { get; private set; }

    [field: SerializeField] public LineRenderer prevLine { get; set; }
    [field: SerializeField] public int pointIndex {  get;  set; }

    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInstantiatingThread.RegisterEvent(InstantiateMainThread);
        GameEvents.ThreadEvents.onInitialiseRope.RegisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.RegisterEvent(AddPositionToLineOnDrag);
        GameEvents.ThreadEvents.onCreatingConnection.RegisterEvent(CreateLineAndApplyPullForceOnConnection);
        GameEvents.ThreadEvents.onEmptyList_DetectingPoints.RegisterEvent(ClearDetectedPointsList);
        GameEvents.ThreadEvents.setThreadInput.RegisterEvent(SetThreadInputBool);
        GameEvents.ThreadEvents.onSetFreeformMovementValue.RegisterEvent(SetFreeformThreadMovement);
        GameEvents.ThreadEvents.onResetThreadInput.RegisterEvent(ResetThread);

    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.UnregisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.UnregisterEvent(AddPositionToLineOnDrag);
        GameEvents.ThreadEvents.onCreatingConnection.UnregisterEvent(CreateLineAndApplyPullForceOnConnection);
        GameEvents.ThreadEvents.onEmptyList_DetectingPoints.UnregisterEvent(ClearDetectedPointsList);
        GameEvents.ThreadEvents.setThreadInput.UnregisterEvent(SetThreadInputBool);
        GameEvents.ThreadEvents.onSetFreeformMovementValue.UnregisterEvent(SetFreeformThreadMovement);
        GameEvents.ThreadEvents.onInstantiatingThread.UnregisterEvent(InstantiateMainThread);
        GameEvents.ThreadEvents.onResetThreadInput.UnregisterEvent(ResetThread);

    }
    public void SetLastConnectedPosition(Transform t)
    {
        lastConnectedPoint = t;
    }
    void SetFreeformThreadMovement(bool value)
    {
        freeForm = value;
    }
    void SetThreadInputBool(bool value)
    {
        threadInput = value;
    }
    void ClearDetectedPointsList()
    {
        detectedPoints.Clear();
    }

    public void SetSpoolId(int id)
    {
        threadIndex = id;
    }
    public void SetThreadColor(int id)
    {
        SetSpoolId(id);
        if (instantiatedLine != null)
        {
            //instantiatedLine.material.color = threadColor[id];
            //instantiatedLine.endColor = threadColor[id];
        }
        if(prevLine != null)
        {
            //prevLine.material.color = threadColor[id];
            //prevLine.endColor = threadColor[id];
        }
    }
    void InstantiateMainThread(bool start, Vector2 startPos)
    {
        instantiatedLine = Instantiate(lineRenderer, threadParent.position, Quaternion.identity);
        instantiatedLine.transform.SetParent(threadParent);
        instantiatedLine.transform.position = Vector3.zero;
        instantiatedLine.positionCount = threadMaxLength;
        //instantiatedLine.material.color = threadColor[threadIndex];
        //instantiatedLine.endColor = threadColor[threadIndex];
        if (start)
        {
            for (int i = 0; i < instantiatedLine.positionCount; i++)
            {
                instantiatedLine.SetPosition(i, startPos);
            }
        }

    }
    public void AddFirstPositionOnMouseDown(Vector2 headPos)
    {
        if (!threadInput) return;
        if (instantiatedLine == null)
        {
            InstantiateMainThread(true, headPos);
            GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.RaiseEvent(true);
            return;
        }
        if (lastConnectedPoint != null)
        {
            currentRopeStartPosition = lastConnectedPoint.position;
            if (freeForm)
                currentRopeStartPosition.z = zVal;
            //instantiatedLine.SetPosition(0, currentRopeStartPosition);
        }
        else
        {
            currentRopeStartPosition = headPos;
            //instantiatedLine.SetPosition(0, headPos);
        }
        //GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(instantiatedLine.GetPosition(0));
    }

    public void AddPositionToLineOnDrag(Vector2 mousePos)
    {
        if (!threadInput) return;
        if (instantiatedLine == null) return;
        Vector3 targetRopePosition = Vector3.zero;

        if (lastConnectedPoint != null && !freeForm)
        {
            currentRopeStartPosition = lastConnectedPoint.position;

            targetRopePosition = new Vector3(mousePos.x, mousePos.y, zVal);

            Vector3 direction = targetRopePosition - currentRopeStartPosition;
            float threadLength = threadMaxLength * minDistanceBetweenSewPoints;

            if (direction.magnitude > threadLength)
                targetRopePosition = currentRopeStartPosition + direction.normalized * threadLength;
        }
        else
        {
            targetRopePosition = new Vector3(mousePos.x, mousePos.y, zVal);
        }

        GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(targetRopePosition);
        Vector3 needlePos = GameEvents.NeedleEvents.OnFetchingNeedlePosition.RaiseEvent();
        needlePos.z = zVal;
        instantiatedLine.SetPosition(0, needlePos);

        if (prevLine)
            MoveThread(prevLine, true);
        MoveThread(instantiatedLine, false);

        prevMouseDragPosition = mousePos;
    }

    public void MoveThread(LineRenderer thread, bool isPrevThread)
    {
        if (!threadInput) return;
        for (int i = 1; i < thread.positionCount; i++)
        {
            Vector3 prevThreadPoint = thread.GetPosition(i - 1);
            Vector3 currentPoint = thread.GetPosition(i);
            Vector3 direction = currentPoint - prevThreadPoint;

            float minDistance = isPrevThread ? 0.001f : minDistanceBetweenSewPoints;
            float lerpSpeed = isPrevThread ? 0.3f : 1.0f;
            if (freeForm)
                lerpSpeed = 1.0f;
            Vector3 targetPos = prevThreadPoint + direction.normalized * minDistance;
            Vector3 lerpPosition = Vector3.Lerp(currentPoint, targetPos, lerpSpeed);
            lerpPosition.z = zVal;
            thread.SetPosition(i, lerpPosition);
        }

        if (!isPrevThread && lastConnectedPoint != null && !freeForm)
        {
            Vector3 lastPos = lastConnectedPoint.position;
            lastPos.z = zVal;
            thread.SetPosition(thread.positionCount - 1, lastPos);

            for (int i = thread.positionCount - 2; i >= 0; i--)
            {
                Vector3 nextPoint = thread.GetPosition(i + 1);
                Vector3 currentPoint = thread.GetPosition(i);
                Vector3 direction = currentPoint - nextPoint;

                float minDistance = minDistanceBetweenSewPoints;
                Vector3 targetPos = nextPoint + direction.normalized * minDistance;
                float smoothFactor = 1.0f;
                Vector3 lerpPosition = Vector3.Lerp(currentPoint, targetPos, smoothFactor);
                lerpPosition.z = zVal;
                thread.SetPosition(i, lerpPosition);
            }
        }
    }

    void CreateLineAndApplyPullForceOnConnection(SewPoint point)
    {
       
        detectedPoints.Add(point.transform);

        detectedPointsCount++;
        Vector3 pos=Vector3.zero;
    
        if (detectedPointsCount % 2 == 0)
        {
            for (int i = 0; i < threadParent.childCount; i++)
            {
                Destroy(threadParent.GetChild(i).gameObject);
            }
        }
        if(detectedPoints.Count > 1)
            pos = detectedPoints[detectedPoints.Count - 2].position;
        else
            pos = point.transform.position;
        pos.z = point.zVal;
        instantiatedLine.SetPosition(0, pos);
        prevLine = instantiatedLine;
        prevLine.name = "Previous Line";
        lastConnectedPoint = point.transform;
        InstantiateMainThread(false, Vector2.zero);

        AddFirstPositionOnMouseDown(point.transform.position);
        for (int i = 0; i < instantiatedLine.positionCount; i++)
            instantiatedLine.SetPosition(i, point.transform.position);
    
        Scale();
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.RaiseEvent(detectedPoints);

    }

    public void ScaleDownAllPoints()
    {
        foreach(Transform t in detectedPoints)
        {
            SewPoint s = t.GetComponent<SewPoint>();
            ScaleInOut(s.transform, 0.2f, 0.25f, false, s.originalScale);
        }
    }
    void Scale()
    {
        if(pointIndex > 0)
        {
            SewPoint s = detectedPoints[pointIndex - 1].GetComponent<SewPoint>();
            ScaleInOut(detectedPoints[pointIndex - 1], 0.2f, 0.25f, false, s.originalScale);
        }
        ScaleInOut(detectedPoints[pointIndex], 0.2f, 0.25f, true, Vector3.zero);
        if (pointIndex < detectedPoints.Count)
            pointIndex++;
        else
            pointIndex = 0;

        CancelInvoke("Scale");
    }
    Tween pointScaleTween = null;
    void ScaleInOut(Transform obj, float targetValue, float speed, bool scaleOut, Vector3 original)
    {
        Vector3 localScale = obj.localScale;
        Vector3 targetScale = Vector3.zero;
        if (scaleOut)
            targetScale = new Vector3(localScale.x + targetValue, localScale.y + targetValue, localScale.z);
        else if (!original.Equals(Vector3.zero))
            targetScale = original;

        pointScaleTween = GameEvents.DoTweenAnimationHandlerEvents.onScaleTransform.Raise(obj, targetScale, speed, Ease.Linear);
        pointScaleTween.OnComplete(() =>
        {
            //pointScaleTween.Kill();
        });
    }
    void ResetThread()
    {
        if(instantiatedLine)
            Destroy(instantiatedLine.gameObject);
        if(prevLine)
            Destroy(prevLine.gameObject);
        lastConnectedPoint = null;
        prevLine = null;
        detectedPointsCount = 0;
        foreach(Transform t in detectedPoints)
        {
            t.gameObject.SetActive(false);
        }
        detectedPoints.Clear();
        prevMouseDragPosition = Vector3.zero;
        GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.RaiseEvent(false);
    }
    public void UndoThread()
    {
        var pointDetector = ServiceLocator.GetService<INeedleDetector>();
        var connectHandler = ServiceLocator.GetService<IPointConnectionHandler>();
     
        if (connectHandler != null)
        {
            if (pointDetector != null)
            {
                if (connectHandler.connections.Count == 0)
                {
                    if(pointDetector.pointsDetected.Count > 0)
                    {
                        SewPoint s = pointDetector.pointsDetected[pointDetector.pointsDetected.Count - 1];
                        s.pointMesh.material = connectHandler.originalMaterial;
                        ScaleInOut(s.transform, 0.2f, 0.25f, false, s.originalScale);
                        if (pointIndex > 0) pointIndex--;
                    }

                }
                pointDetector.UndoLastConnectedPoint();
            }
            if (prevLine)
                Destroy(prevLine.gameObject);

            if (detectedPoints.Count > 0)
            {
                SewPoint s = detectedPoints[(detectedPoints.Count - 1)].GetComponent<SewPoint>();
                if((detectedPoints.Count - 1) > 0)
                {
                    SewPoint s2 = detectedPoints[(detectedPoints.Count - 2)].GetComponent<SewPoint>();
                    if (!s2.connected)
                        ScaleInOut(detectedPoints[(detectedPoints.Count - 2)], 0.2f, 0.25f, true, Vector3.zero);
                }
                ScaleInOut(detectedPoints[(detectedPoints.Count - 1)], 0.2f, 0.25f, false, s.originalScale);
                if (pointIndex > 0) pointIndex--;
                detectedPoints.Remove(detectedPoints[(detectedPoints.Count - 1)]);

                if ((detectedPoints.Count - 1) >= 0)
                    SetLastConnectedPosition(detectedPoints[(detectedPoints.Count - 1)].transform);
                else
                    SetLastConnectedPosition(null);
            }
            else
                SetLastConnectedPosition(null);

           

            if (connectHandler.points.Count > 0)
                connectHandler.points.Remove(connectHandler.points[connectHandler.points.Count - 1]);
            if (connectHandler.connections.Count > 0)
            {
                Connections c = null;
                c = connectHandler.connections[connectHandler.connections.Count - 1];
                connectHandler.connections.Remove(c);
                Transform moveableTransform = null;
                ObjectInfo o_Info1 = null;
                ObjectInfo o_Info2 = null;
                o_Info1 = c.point1.parent.parent.GetComponent<ObjectInfo>();
                o_Info2 = c.point2.parent.parent.GetComponent<ObjectInfo>();
              
                SewPoint s1 = c.point1.GetComponent<SewPoint>();
                SewPoint s2 = c.point2.GetComponent<SewPoint>();
                if (LevelsHandler.instance.currentLevelMeta.noOfCorrectLinks > 0)
                {
                    LevelsHandler.instance.currentLevelMeta.noOfCorrectLinks--;
                    PlayerPrefs.SetInt("StitchedCount", LevelsHandler.instance.currentLevelMeta.noOfCorrectLinks);
                }
                if(lastConnectedPoint != null)
                {
                    for (int i = 0; i < instantiatedLine.positionCount; i++)
                    {
                        instantiatedLine.SetPosition(i, lastConnectedPoint.position);
                    }
                    GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(lastConnectedPoint.position);
                }
                connectHandler.UpdateConnections();

                if (s1.attachmentId.Equals(s2.attachmentId))
                {
                    s1.connected = false;
                    s2.connected = false;
                    if (connectHandler.wrongConnectPoint.Count == 0)
                    {
                        if (o_Info1.noOfConnections > 0)
                            o_Info1.noOfConnections--;
                        if (o_Info2.noOfConnections > 0)
                            o_Info2.noOfConnections--;

                        if (o_Info1.moveable)
                        {
                            if (o_Info1.head)
                                moveableTransform = o_Info1.transform.parent;
                            else
                                moveableTransform = o_Info1.transform;
                            if (o_Info1.movedPositions.Count > 0)
                            {
                                moveableTransform.DOMove(o_Info1.movedPositions[o_Info1.movedPositions.Count - 1], 0.25f).SetEase(Ease.InOutSine).OnUpdate(() =>
                                {
                                    //instantiatedLine.SetPosition(0, lastConnectedPoint.position);
                                    //instantiatedLine.SetPosition((instantiatedLine.positionCount - 1), lastConnectedPoint.position);
                                    //GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(lastConnectedPoint.position);
                                    //connectHandler.UpdateConnections();
                                }).OnComplete(() =>
                                {

                                    moveableTransform.DOPause();
                                });
                                o_Info1.movedPositions.Remove(o_Info1.movedPositions[o_Info1.movedPositions.Count - 1]);
                                if (o_Info2.movedPositions.Count > 0)
                                    o_Info2.movedPositions.Remove(o_Info2.movedPositions[o_Info2.movedPositions.Count - 1]);

                            }

                        }
                        else if (o_Info2.moveable)
                        {
                            if (o_Info2.head)
                                moveableTransform = o_Info2.transform.parent;
                            else
                                moveableTransform = o_Info2.transform;

                            if (o_Info2.movedPositions.Count > 0)
                            {
                                moveableTransform.DOMove(o_Info2.movedPositions[o_Info2.movedPositions.Count - 1], 0.25f).SetEase(Ease.InOutSine).OnUpdate(() =>
                                {
                                    //instantiatedLine.SetPosition(0, lastConnectedPoint.position);
                                    //instantiatedLine.SetPosition((instantiatedLine.positionCount - 1), lastConnectedPoint.position);
                                    //GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(lastConnectedPoint.position);
                                    //connectHandler.UpdateConnections();

                                }).OnComplete(() =>
                                {

                                    moveableTransform.DOPause();
                                });
                                if (o_Info1.movedPositions.Count > 0)
                                    o_Info1.movedPositions.Remove(o_Info1.movedPositions[o_Info1.movedPositions.Count - 1]);
                                o_Info2.movedPositions.Remove(o_Info2.movedPositions[o_Info2.movedPositions.Count - 1]);
                            }

                        }
                    }
                
                    LevelsHandler.instance.currentLevelMeta.UpdateAllStitchesOfPlushie();
                    
                }
              
                if(!s1.connected)
                    s1.pointMesh.material = connectHandler.originalMaterial;
                s2.pointMesh.material = connectHandler.originalMaterial;

                if (connectHandler.wrongConnectPoint.Count > 0)
                {
                    if (connectHandler.wrongConnectPoint.Contains(s2))
                        connectHandler.wrongConnectPoint.Remove(s2);
                }
               
                Destroy(c.line.gameObject);
            }
            else
                connectHandler.points.Clear();

            if (connectHandler.wrongConnectPoint.Count == 0)
            {
                connectHandler.GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.levelParts);
                connectHandler.GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.immoveablePart.GetComponent<Part_Info>().joints);
                connectHandler.GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.head.joints);
            }
        }
    }
   
}
