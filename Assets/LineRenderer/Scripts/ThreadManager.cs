using DG.Tweening;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ThreadManager : MonoBehaviour, IThreadManager
{
    [field: SerializeField] public bool threadInput { get; private set; }
    [field: SerializeField] public bool freeForm { get; private set; }

    [field: SerializeField] public List<Transform> detectedPoints {  get; private set; }

    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] Vector3 currentRopeStartPosition;
    Vector2 prevMouseDragPosition;

    [SerializeField] float minDistanceBetweenSewPoints;
    [SerializeField] int threadMaxLength;

    [SerializeField] private Transform threadParent;
    [SerializeField] int detectedPointsCount = 0;

    [SerializeField]LineRenderer instantiatedLine;
    [SerializeField] Transform pointsLinkParent;
    [SerializeField] LineRenderer prevLine;
    [SerializeField] Transform lastConnectedPoint;
    [SerializeField] float zVal;
    [SerializeField] Transform startPoint;
    private void OnEnable()
    {
        RegisterService();

        //InstantiateMainThread(true, startPoint.position);

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
            GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(instantiatedLine.GetPosition(0));
        }

    }

  
    public void AddFirstPositionOnMouseDown(Vector2 headPos)
    {

        if (!threadInput) return;
        if (instantiatedLine == null)
        {
            InstantiateMainThread(true, headPos);
            return;
        }
        if (lastConnectedPoint != null)
        {
            currentRopeStartPosition = lastConnectedPoint.position;
            if (freeForm)
                currentRopeStartPosition.z = zVal;
            instantiatedLine.SetPosition(0, currentRopeStartPosition);
        }
        else
        {
            currentRopeStartPosition = headPos;
            instantiatedLine.SetPosition(0, headPos);
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

        instantiatedLine.SetPosition(0, targetRopePosition);
        GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(instantiatedLine.GetPosition(0));
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.RaiseEvent(detectedPoints);
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

            float minDistance = isPrevThread ? 0f : minDistanceBetweenSewPoints;
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
    }

}
