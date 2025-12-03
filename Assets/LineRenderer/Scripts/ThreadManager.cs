using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour, IThreadManager
{
    [field: SerializeField] public bool threadInput { get; private set; }
    [field: SerializeField] public List<Transform> detectedPoints {  get; private set; }

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
    //[SerializeField] float zVal;
    //[SerializeField] Transform startPoint;
    [SerializeField] RectTransform startUIPoint;
    [SerializeField] Vector3 direction;
    [field: SerializeField] public LineRenderer prevLine { get; set; }
    [field: SerializeField] public int pointIndex {  get;  set; }

    [field: SerializeField] public float zVal {  get; private set; }
    [field: SerializeField] public bool canUndo {  get; private set; }

    Vector3 startPos;
    private void OnEnable()
    {
        canUndo = true;
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
        GameEvents.ThreadEvents.onInstantiatingThread.UnregisterEvent(InstantiateMainThread);
        GameEvents.ThreadEvents.onResetThreadInput.UnregisterEvent(ResetThread);

    }
    public void SetUndoValue(bool val)
    {
        canUndo = val;
    }
    public void SetLastConnectedPosition(Transform t)
    {
        lastConnectedPoint = t;
    }
  
    void SetThreadInputBool(bool value)
    {
        threadInput = value;
    }
    void ClearDetectedPointsList()
    {
        detectedPoints.Clear();
    }
    public void ResetList(List<Transform> list)
    {
        detectedPoints = list;
    }
    public void SetThreadColor()
    {
        if (instantiatedLine != null)
        {
            instantiatedLine.material.color = LevelsHandler.instance.currentLevelMeta.levelScriptable.threadColor;
            //instantiatedLine.endColor = LevelsHandler.instance.currentLevelMeta.threadColor;
        }
        if(prevLine != null)
        {
            prevLine.material.color = LevelsHandler.instance.currentLevelMeta.levelScriptable.threadColor;
            //prevLine.endColor = LevelsHandler.instance.currentLevelMeta.threadColor;
        }
    }
    void InstantiateMainThread(bool start, Vector2 startPos)
    {
        instantiatedLine = Instantiate(lineRenderer, threadParent.position, Quaternion.identity);
        instantiatedLine.transform.SetParent(threadParent);
        instantiatedLine.transform.position = Vector3.zero;
        instantiatedLine.positionCount = threadMaxLength;
        if (start)
        {
            for (int i = 0; i < instantiatedLine.positionCount; i++)
            {
                instantiatedLine.SetPosition(i, startPos);
            }
        }
        SetThreadColor();
    }
    public void AddFirstPositionOnMouseDown(Vector2 headPos)
    {
        if (!threadInput) return;
        UpdateStartPositionFromSpool();
        if (instantiatedLine == null)
        {
            InstantiateMainThread(true, startPos);
            GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.RaiseEvent(true);
            return;
        }
        if (lastConnectedPoint != null)
        {
            currentRopeStartPosition = lastConnectedPoint.position;
        }
        else
        {
            currentRopeStartPosition = startPos;
        }
    }
    void UpdateStartPositionFromSpool()
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, startUIPoint.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        startPos = worldPos;
        startPos.z = zVal;
    }
    public void AddPositionToLineOnDrag(Vector2 mousePos)
    {
        if (!threadInput) return;
        if (instantiatedLine == null) return;
        UpdateStartPositionFromSpool();

        Vector3 targetRopePosition = Vector3.zero;

        if (lastConnectedPoint != null)
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
        instantiatedLine.SetPosition(0, targetRopePosition);
        Vector3 needlePos = (instantiatedLine.GetPosition(0))/* + instantiatedLine.GetPosition(1))/2*/;
        needlePos.z = zVal;
       
        GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(needlePos);

        if (prevLine)
        {
            prevLine.name = "First Line";
            MoveThread(prevLine, true);
        }
        MoveThread(instantiatedLine, false);
        prevMouseDragPosition = mousePos;
    }
    public void MoveThread(LineRenderer thread, bool isPrevThread)
    {
        if (!threadInput) return;

        int threadPositionCount = thread.positionCount;
        Vector3[] positions = new Vector3[threadPositionCount];
        thread.GetPositions(positions);
        positions[threadPositionCount - 1] = startPos;

        float minDistance = isPrevThread ? 0.001f : minDistanceBetweenSewPoints;
        float lerpSpeed = isPrevThread ? 0.3f : 1.0f;

        for (int i = 1; i < threadPositionCount - 1; i++)
        {
            Vector3 direction = positions[i] - positions[i - 1];
            Vector3 targetPos = positions[i - 1] + direction.normalized * minDistance;
            positions[i] = Vector3.Lerp(positions[i], targetPos, lerpSpeed);
            positions[i].z = zVal;
        }

        if (!isPrevThread)
        {
            Vector3 endPos = lastConnectedPoint ? lastConnectedPoint.position : startPos;
            endPos.z = zVal;
            positions[threadPositionCount - 1] = endPos;

            SmoothThreadBackward(positions, minDistanceBetweenSewPoints, 1.0f);
            thread.name = lastConnectedPoint ? "Current" : thread.name;

            thread.SetPositions(positions);

            if (lastConnectedPoint && prevLine != null)
            {
                prevLine.positionCount = threadMaxLength / 5;
                UpdatePrevLine(prevLine, detectedPoints[0].position, startPos, 0.3f);
            }
        }
        else
            thread.SetPositions(positions);
    }

    private void SmoothThreadBackward(Vector3[] positions, float minDistance, float smoothFactor)
    {
        for (int i = positions.Length - 2; i >= 0; i--)
        {
            Vector3 direction = positions[i] - positions[i + 1];
            Vector3 targetPos = positions[i + 1] + direction.normalized * minDistance;
            positions[i] = Vector3.Lerp(positions[i], targetPos, smoothFactor);
            positions[i].z = zVal;
        }
    }
    public void UpdateSpoolThreadLastPoint(float lerpSpeed)
    {
        if(prevLine)
            UpdatePrevLine(prevLine, detectedPoints[0].position, startPos, lerpSpeed);
        if (instantiatedLine)
            instantiatedLine.SetPosition(instantiatedLine.positionCount - 1, lastConnectedPoint.position);
    }
    private void UpdatePrevLine(LineRenderer line, Vector3 start, Vector3 end, float lerpSpeed)
    {
        int count = line.positionCount;
        Vector3[] positions = new Vector3[count];
        line.GetPositions(positions);

        positions[0] = start;
        positions[count - 1] = end;

        SmoothThreadBackward(positions, minDistanceBetweenSewPoints, lerpSpeed);
        line.SetPositions(positions);

    }
    void CreateLineAndApplyPullForceOnConnection(SewPoint point)
    {
       
        detectedPoints.Add(point.transform);

        detectedPointsCount++;
        Vector3 pos=Vector3.zero;
    
        if(detectedPoints.Count > 1)
            pos = detectedPoints[detectedPoints.Count - 2].position;
        else
            pos = point.transform.position;
        pos.z = point.zVal;
        instantiatedLine.SetPosition(0, pos);
       
        lastConnectedPoint = point.transform;
        if(detectedPoints.Count == 1)
        {
            prevLine = instantiatedLine;
            InstantiateMainThread(false, Vector2.zero);
        }
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
        if (!canUndo) return;
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
                        //s.pointMesh.material = connectHandler.originalMaterial;
                        ScaleInOut(s.transform, 0.2f, 0.25f, false, s.originalScale);
                        if (pointIndex > 0) pointIndex--;
                    }

                }

                pointDetector.UndoLastConnectedPoint();
            }
        

            if (detectedPoints.Count > 0)
            {
                if (prevLine != null)
                    prevLine.SetPosition(0, detectedPoints[0].position);
                SewPoint s = detectedPoints[(detectedPoints.Count - 1)].GetComponent<SewPoint>();
               s.pointMesh.material = connectHandler.originalMaterial;
                if (connectHandler.wrongConnectPoint.Count > 0)
                {
                    if (connectHandler.wrongConnectPoint.Contains(s))
                        connectHandler.wrongConnectPoint.Remove(s);
                    connectHandler.UpdateColorOfPoints();
                }

                if ((detectedPoints.Count - 1) > 0)
                {
                    SewPoint s2 = detectedPoints[(detectedPoints.Count - 2)].GetComponent<SewPoint>();
                    if (!s2.metaData.connected)
                        ScaleInOut(detectedPoints[(detectedPoints.Count - 2)], 0.2f, 0.25f, true, Vector3.zero);
                }
                ScaleInOut(detectedPoints[(detectedPoints.Count - 1)], 0.2f, 0.25f, false, s.originalScale);
                if (pointIndex > 0) pointIndex--;
                detectedPoints.Remove(detectedPoints[(detectedPoints.Count - 1)]);

                if ((detectedPoints.Count - 1) >= 0)
                {
                    SewPoint s2 = null;
                    if((detectedPoints.Count - 2) >= 0)
                        s2 = detectedPoints[(detectedPoints.Count - 2)].GetComponent<SewPoint>();
                    SewPoint s1 = detectedPoints[(detectedPoints.Count - 1)].GetComponent<SewPoint>();
                    if(s2 != null)
                    {
                        if (s2.nextConnectedPointId.Equals(s1.attachmentId))
                        {
                            if (s2.startFlag)
                            {
                                if(s2.transform.parent.parent.parent != s1.transform.parent.parent.parent)
                                {
                                    if (s2.metaData.connected)
                                        s1.pointMesh.material = connectHandler.correctPointMaterial;
                                    else
                                        s1.pointMesh.material = connectHandler.wrongPointMaterial;
                                }
                                else
                                {
                                    if (s2.metaData.connected)
                                        s1.pointMesh.material = connectHandler.correctPointMaterial;
                                    else
                                        s1.pointMesh.material = connectHandler.wrongPointMaterial;
                                }

                            }
                            else
                            {
                                if (s2.transform.parent.parent.parent != s1.transform.parent.parent.parent)
                                {
                                    if (s2.metaData.connected)
                                        s1.pointMesh.material = connectHandler.correctPointMaterial;
                                    else
                                        s1.pointMesh.material = connectHandler.wrongPointMaterial;
                                }
                                else
                                {
                                    if (s2.metaData.connected)
                                        s1.pointMesh.material = connectHandler.correctPointMaterial;
                                    else
                                        s1.pointMesh.material = connectHandler.wrongPointMaterial;
                                }
                            }
                        }
                    }
                    else
                    {
                        if(s1!= null)
                        {
                            if(s1.startFlag) s1.pointMesh.material = connectHandler.correctPointMaterial;
                        }
                    }
                    SetLastConnectedPosition(detectedPoints[(detectedPoints.Count - 1)].transform);
                }
                else
                    SetLastConnectedPosition(null);

         
            }
            else
                SetLastConnectedPosition(null);
            if (prevLine && detectedPoints.Count == 0)
            {
                Destroy(prevLine.gameObject);
                Destroy(instantiatedLine.gameObject);
                GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(startPos);
            }

            if (connectHandler.points.Count > 0)
                connectHandler.points.Remove(connectHandler.points[connectHandler.points.Count - 1]);
            if (connectHandler.connections.Count > 0)
            {
                Connections c = null;
                c = connectHandler.connections[connectHandler.connections.Count - 1];
                connectHandler.connections.Remove(c);
                if (LevelsHandler.instance.currentLevelMeta.noOfStitchesDone > 0)
                {
                    LevelsHandler.instance.currentLevelMeta.noOfStitchesDone--;
                    if (LevelsHandler.instance.currentLevelMeta.currentSpool)
                    {
                        SpoolInfo s_Info = LevelsHandler.instance.currentLevelMeta.currentSpool.GetComponent<SpoolInfo>();
                        s_Info.noOfStitchedDone--;
                    }
                    PlayerPrefs.SetInt("StitchedCount", LevelsHandler.instance.currentLevelMeta.noOfStitchesDone);
                    var canvasManager = ServiceLocator.GetService<ICanvasUIManager>();
                    if (canvasManager != null)
                        canvasManager.UpdateStitchCount(LevelsHandler.instance.currentLevelMeta.levelScriptable.totalStitches, LevelsHandler.instance.currentLevelMeta.noOfStitchesDone);
                }

                Transform moveableTransform = null;
                ObjectInfo o_Info1 = null;
                ObjectInfo o_Info2 = null;
                o_Info1 = c.point1.parent.parent.GetComponent<ObjectInfo>();
                o_Info2 = c.point2.parent.parent.GetComponent<ObjectInfo>();
              
                SewPoint s1 = c.point1.GetComponent<SewPoint>();
                SewPoint s2 = c.point2.GetComponent<SewPoint>();
               
                if(lastConnectedPoint != null)
                {
                    if(instantiatedLine != null)
                    {
                        for (int i = 1; i < instantiatedLine.positionCount; i++)
                        {
                            instantiatedLine.SetPosition(i, lastConnectedPoint.position);
                        }
                    }
                 
                    //GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(lastConnectedPoint.position);
                }
                connectHandler.UpdateConnections();

                if (s1.attachmentId.Equals(s2.attachmentId))
                {
                    if(s1.metaData.connected)
                    {
                        if(LevelsHandler.instance.currentLevelMeta.cleanConnection.Count > 0)
                        {
                            Connections c_Clean = LevelsHandler.instance.currentLevelMeta.cleanConnection[LevelsHandler.instance.currentLevelMeta.cleanConnection.Count - 1];
                            LevelsHandler.instance.currentLevelMeta.cleanConnection.Remove(c_Clean);
                        }
                     
                    }


                    if (connectHandler.wrongConnectPoint.Count == 0)
                    {
                        if (o_Info1.stitchData.noOfConnections > 0)
                            o_Info1.DecementConnection();
                        if (o_Info2.stitchData.noOfConnections > 0)
                            o_Info2.DecementConnection();

                        if (o_Info1.moveable)
                        {
                            if (o_Info1.head)
                                moveableTransform = o_Info1.transform.parent;
                            else
                                moveableTransform = o_Info1.transform;
                            if(o_Info1.stitchData != null)
                            {
                                if (o_Info1.stitchData.movedPositions.Count > 0)
                                {
                                    moveableTransform.DOMove(o_Info1.stitchData.movedPositions[o_Info1.stitchData.movedPositions.Count - 1], 0.25f).SetEase(Ease.InOutSine).OnUpdate(() =>
                                    {
                                        if (lastConnectedPoint != null)
                                        {
                                            if(instantiatedLine != null)
                                            {
                                                for (int i = 1; i < instantiatedLine.positionCount; i++)
                                                {
                                                    instantiatedLine.SetPosition(i, lastConnectedPoint.position);
                                                }

                                            }

                                            //GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(lastConnectedPoint.position);
                                        }
                                        connectHandler.UpdateConnections();
                                    }).OnComplete(() =>
                                    {

                                        moveableTransform.DOPause();
                                    });
                                    o_Info1.stitchData.movedPositions.Remove(o_Info1.stitchData.movedPositions[o_Info1.stitchData.movedPositions.Count - 1]);
                                    if (o_Info2.stitchData != null)
                                    {
                                        if (o_Info2.stitchData.movedPositions.Count > 0)
                                            o_Info2.stitchData.movedPositions.Remove(o_Info2.stitchData.movedPositions[o_Info2.stitchData.movedPositions.Count - 1]);
                                    }


                                }
                            }
                            

                        }
                        else if (o_Info2.moveable)
                        {
                            if (o_Info2.head)
                                moveableTransform = o_Info2.transform.parent;
                            else
                                moveableTransform = o_Info2.transform;

                            if(o_Info2.stitchData != null)
                            {
                                if (o_Info2.stitchData.movedPositions.Count > 0)
                                {
                                    moveableTransform.DOMove(o_Info2.stitchData.movedPositions[o_Info2.stitchData.movedPositions.Count - 1], 0.25f).SetEase(Ease.InOutSine).OnUpdate(() =>
                                    {
                                        if (lastConnectedPoint != null)
                                        {
                                            if(instantiatedLine != null)
                                            {
                                                for (int i = 1; i < instantiatedLine.positionCount; i++)
                                                {
                                                    instantiatedLine.SetPosition(i, lastConnectedPoint.position);
                                                }
                                            }
                                         

                                            //GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(lastConnectedPoint.position);
                                        }
                                        connectHandler.UpdateConnections();

                                    }).OnComplete(() =>
                                    {

                                        moveableTransform.DOPause();
                                    });
                                    if(o_Info1.stitchData != null)
                                    {
                                        if (o_Info1.stitchData.movedPositions.Count > 0)
                                            o_Info1.stitchData.movedPositions.Remove(o_Info1.stitchData.movedPositions[o_Info1.stitchData.movedPositions.Count - 1]);
                                    }
                                    if(o_Info2.stitchData != null)
                                        o_Info2.stitchData.movedPositions.Remove(o_Info2.stitchData.movedPositions[o_Info2.stitchData.movedPositions.Count - 1]);
                                }
                            }
                           

                        }
                    }
                    if(o_Info1.stitchData != null)
                    {
                        if (o_Info1.stitchData.movedPositions.Count > 0)
                            s1.IsConnected(false, 0, o_Info1.stitchData.movedPositions[o_Info1.stitchData.movedPositions.Count - 1], o_Info1.partType.ToString());
                        else
                            s1.IsConnected(false, 0, Vector3.zero, "");

                        if (o_Info2.stitchData.movedPositions.Count > 0)
                            s2.IsConnected(false, 0, o_Info2.stitchData.movedPositions[o_Info2.stitchData.movedPositions.Count - 1], o_Info2.partType.ToString());
                        else
                            s2.IsConnected(false, 0, Vector3.zero, "");
                    }



                    SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + o_Info1.partType, o_Info1.stitchData);
                    SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + o_Info2.partType, o_Info2.stitchData);
                    
                    LevelsHandler.instance.currentLevelMeta.UpdateAllStitchesOfPlushie();
                    
                }
               
                Destroy(c.line.gameObject);
            }
            else
                connectHandler.points.Clear();

            if (connectHandler.wrongConnectPoint.Count == 0)
            {
                connectHandler.GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.bodyParts);
                connectHandler.GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.immoveablePart.GetComponent<Part_Info>().joints);
                connectHandler.GetObjectInfoWrongAlertTextDisableOfPart(LevelsHandler.instance.currentLevelMeta.head.joints);
                var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
                if (canvasHandler != null)
                    canvasHandler.undoHighLight.SetActive(false);
            }
            if(detectedPoints.Count > 0)
                LevelsHandler.instance.currentLevelMeta.ResetNeedlePosition(LevelsHandler.instance.currentLevelMeta.currentActivePart);
     
        }
    }

    public void UpdateCurrentActiveSpoolReference()
    {
        int indexOfSpool = LevelsHandler.instance.currentLevelMeta.currentActiveSpoolIndex;
        var IspoolHandler = ServiceLocator.GetService<ISpoolManager>();
        if(IspoolHandler != null)
        {
            GameObject currentSpool = IspoolHandler.GetSpool(indexOfSpool);
            startUIPoint = currentSpool.GetComponent<RectTransform>();
        }
    }
}
