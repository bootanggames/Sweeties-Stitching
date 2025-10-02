using System.Collections.Generic;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class ThreadManager : MonoBehaviour,IThreadManager
{
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] Vector3 currentRopeStartPosition;
    Vector2 prevMouseDragPosition;

    [SerializeField] float minDistanceBetweenSewPoints;
    [SerializeField] int threadMaxLength;

    [SerializeField] private Transform threadParent;
    [SerializeField] List<Transform> detectedPoints = new List<Transform>();
    [SerializeField] int detectedPointsCount = 0;

    [SerializeField]LineRenderer instantiatedLine;
    [SerializeField] Transform pointsLinkParent;
    LineRenderer prevLine;
    Transform lastConnectedPoint;
    private void OnEnable()
    {
        InstantiateMainThread();
        RegisterService();

    }

    void InstantiateMainThread()
    {
        if (instantiatedLine != null)
            Destroy(instantiatedLine.gameObject);
        instantiatedLine = Instantiate(lineRenderer, threadParent.position, Quaternion.identity);
        instantiatedLine.transform.SetParent(threadParent);
        instantiatedLine.transform.position = Vector3.zero;

        instantiatedLine.positionCount = threadMaxLength;
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
 
    public void AddFirstPositionOnMouseDown(Vector2 headPos)
    {
        if (instantiatedLine == null) return;
        instantiatedLine.SetPosition(0, headPos);
        currentRopeStartPosition = instantiatedLine.GetPosition(0);
    }

  
    public void AddPositionToLineOnDrag(Vector2 mousePos)
    {
        if (instantiatedLine == null) return;

        Vector3 targetRopePosition = new Vector3(mousePos.x, mousePos.y, 0);
        if(lastConnectedPoint!= null) targetRopePosition = lastConnectedPoint.position;
        Vector3 direction = targetRopePosition - currentRopeStartPosition;
        float threadLength = threadMaxLength * minDistanceBetweenSewPoints;

        if (direction.magnitude > threadLength)
            targetRopePosition = currentRopeStartPosition + direction.normalized * threadLength;

        instantiatedLine.SetPosition(0, targetRopePosition);
        if(prevLine)
            MoveThread(prevLine, true);
        else
            MoveThread(instantiatedLine, false);
        prevMouseDragPosition = mousePos;
    }

    public void MoveThread(LineRenderer thread, bool isPrevThread)
    {
        if (prevLine == null && lastConnectedPoint == null)
        {
            for (int i = 1; i < thread.positionCount; i++)
            {
                Vector3 prevThreadPoint = thread.GetPosition(i - 1);
                Vector3 currentPoint = thread.GetPosition(i);
                Vector3 direction = currentPoint - prevThreadPoint;

                float minDistance = isPrevThread ? 0f : minDistanceBetweenSewPoints;
                float lerpSpeed = isPrevThread ? 0.3f : 1.0f;
                if (direction.magnitude > minDistance)
                {
                    Vector3 targetPos = prevThreadPoint + direction.normalized * minDistance;
                    Vector3 lerpPosition = Vector3.Lerp(currentPoint, targetPos, lerpSpeed);
                    thread.SetPosition(i, lerpPosition);
                }
                else
                {
                    thread.SetPosition(i, currentPoint);
                }
            }
        }
        else
        {
            thread.SetPosition((thread.positionCount - 1), lastConnectedPoint.transform.position);
            for (int i = thread.positionCount - 2; i >= 0; i--)
            {
                Vector3 nextPointPosition = thread.GetPosition(i + 1);
                Vector3 currentPoint = thread.GetPosition(i);
                Vector3 direction = currentPoint - nextPointPosition;
                float minDistance = isPrevThread ? 0f : minDistanceBetweenSewPoints;
                float lerpSpeed = isPrevThread ? 0.3f : 1.0f;
                if (direction.magnitude > minDistance)
                {
                    Vector3 targetPos = nextPointPosition + direction.normalized * minDistance;
                    Vector3 lerpPosition = Vector3.Lerp(currentPoint, targetPos, lerpSpeed);
                    thread.SetPosition(i, lerpPosition);
                }
            }
        }
      
    }
   
    void CreateLineAndApplyPullForceOnConnection(SewPoint point)
    {
        detectedPoints.Add(point.transform);

        detectedPointsCount++;
        LineRenderer line = null;
        if (detectedPointsCount > 1)
        {
            for (int i = 0; i < threadParent.childCount; i++)
            {
                Destroy(threadParent.GetChild(i).gameObject);
            }
            line = Instantiate(lineRenderer, pointsLinkParent.position, Quaternion.identity);
            line.transform.name = "Link";
            line.positionCount = detectedPointsCount;
            line.transform.SetParent(pointsLinkParent);
            line.transform.position = Vector3.zero;
            for (int i=0;i<detectedPointsCount; i++)
            {
                line.SetPosition(i,detectedPoints[i].position);
            }
            detectedPointsCount = 0;
            detectedPoints.Clear();
        }
        else
        {
            prevLine = instantiatedLine;
        }
        InstantiateMainThread();
        instantiatedLine.SetPosition(0, point.transform.position);
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.RaiseEvent(point, line);
        currentRopeStartPosition = point.transform.position;
        lastConnectedPoint = point.transform;
        for (int i = 0; i < instantiatedLine.positionCount; i++)
            instantiatedLine.SetPosition(i, point.transform.position);
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.RegisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.RegisterEvent(AddPositionToLineOnDrag);
        GameEvents.ThreadEvents.onCreatingConnection.RegisterEvent(CreateLineAndApplyPullForceOnConnection);

    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.UnregisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.UnregisterEvent(AddPositionToLineOnDrag);
        GameEvents.ThreadEvents.onCreatingConnection.UnregisterEvent(CreateLineAndApplyPullForceOnConnection);

    }


}
