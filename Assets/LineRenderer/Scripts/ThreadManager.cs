using DG.Tweening;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
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
    [SerializeField] LineRenderer prevLine;
    [SerializeField] Transform lastConnectedPoint;
    [SerializeField] float zVal;
    [SerializeField] Transform startPoint;
    private void OnEnable()
    {
        InstantiateMainThread();
        RegisterService();

    }
    void ClearDetectedPointsList()
    {
        detectedPoints.Clear();
    }
    void InstantiateMainThread()
    {
        instantiatedLine = Instantiate(lineRenderer, threadParent.position, Quaternion.identity);
        instantiatedLine.transform.SetParent(threadParent);
        instantiatedLine.transform.position = Vector3.zero;

        instantiatedLine.positionCount = threadMaxLength;
        //for(int i = 0; i < instantiatedLine.positionCount; i++)
        //{
        //    instantiatedLine.SetPosition(i, startPoint.position);
        //}
    }
    private void OnDisable()
    {
        UnRegisterService();
    }

    public void AddFirstPositionOnMouseDown(Vector2 headPos)
    {
        if (instantiatedLine == null) return;

        if (lastConnectedPoint != null)
        {
            currentRopeStartPosition = lastConnectedPoint.position;
            instantiatedLine.SetPosition(0, currentRopeStartPosition);
        }
        else
        {
            currentRopeStartPosition = headPos;
            instantiatedLine.SetPosition(0, headPos);
        }
    }

    public void AddPositionToLineOnDrag(Vector2 mousePos)
    {
        if (instantiatedLine == null) return;

        Vector3 targetRopePosition;

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
        GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(instantiatedLine.GetPosition(0));
        GameEvents.PointConnectionHandlerEvents.onFetchingPoints.RaiseEvent(detectedPoints);
        if (prevLine)
        {
            MoveThread(prevLine, true);
        }
        else
        {
           
            MoveThread(instantiatedLine, false);

        }
     
        prevMouseDragPosition = mousePos;

    }


    public void MoveThread(LineRenderer thread, bool isPrevThread)
    {
      

        for (int i = 1; i < thread.positionCount; i++)
        {
            Vector3 prevThreadPoint = thread.GetPosition(i - 1);
            Vector3 currentPoint = thread.GetPosition(i);
            Vector3 direction = currentPoint - prevThreadPoint;

            float minDistance = isPrevThread ? 0f : minDistanceBetweenSewPoints;
            float lerpSpeed = isPrevThread ? 0.3f : 1.0f;

            Vector3 targetPos = prevThreadPoint + direction.normalized * minDistance;
            Vector3 lerpPosition = Vector3.Lerp(currentPoint, targetPos, lerpSpeed);
            lerpPosition.z = zVal;

            thread.SetPosition(i, lerpPosition);
        }

        if (!isPrevThread && lastConnectedPoint != null)
        {
            Vector3 lastPos = lastConnectedPoint.position;
            lastPos.z = -0.3f;
            thread.SetPosition(thread.positionCount - 1, lastPos);

            for (int i = thread.positionCount - 2; i >= 0; i--)
            {
                Vector3 nextPoint = thread.GetPosition(i + 1);
                Vector3 currentPoint = thread.GetPosition(i);
                Vector3 direction = currentPoint - nextPoint;

                float minDistance = minDistanceBetweenSewPoints;
                Vector3 targetPos = nextPoint + direction.normalized * minDistance;
                Vector3 lerpPosition = Vector3.Lerp(currentPoint, targetPos, 1f);
                lerpPosition.z = zVal;

                thread.SetPosition(i, lerpPosition);
            }
        }
    }
    LineRenderer link = null;
    float tweenDuration = 1.5f;
    void CreateLineAndApplyPullForceOnConnection(SewPoint point)
    {
        detectedPoints.Add(point.transform);

        detectedPointsCount++;
        Vector3 pos=Vector3.zero;

        pos = point.transform.position;
        pos.z = zVal;
    
        if (detectedPointsCount % 2 == 0)
        {
            for (int i = 0; i < threadParent.childCount; i++)
            {
                Destroy(threadParent.GetChild(i).gameObject);
            }
        }
        instantiatedLine.SetPosition(0, pos);
        prevLine = instantiatedLine;

        prevLine.name = "Previous Line";
        lastConnectedPoint = point.transform;
        InstantiateMainThread();

        AddFirstPositionOnMouseDown(pos);

        for (int i = 0; i < instantiatedLine.positionCount; i++)
            instantiatedLine.SetPosition(i, pos);
    }

    void UpdateLink()
    {
        if (detectedPoints.Count < 2 || link == null)
            return;

        Transform p1 = detectedPoints[detectedPoints.Count - 2];
        Transform p2 = detectedPoints[detectedPoints.Count - 1];


        if (link != null)
        {
            Vector3 p1_pos = p1.position;
            p1_pos.z = zVal;
            link.SetPosition(0, p1_pos);
            Vector3 p2_pos = p2.position;
            p2_pos.z = zVal;
            link.SetPosition(1, p2_pos);
        }
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.RegisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.RegisterEvent(AddPositionToLineOnDrag);
        GameEvents.ThreadEvents.onCreatingConnection.RegisterEvent(CreateLineAndApplyPullForceOnConnection);
        GameEvents.ThreadEvents.onUpdateLinkMovement.RegisterEvent(UpdateLink);
        GameEvents.ThreadEvents.onEmptyList_DetectingPoints.RegisterEvent(ClearDetectedPointsList);

    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.UnregisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.UnregisterEvent(AddPositionToLineOnDrag);
        GameEvents.ThreadEvents.onCreatingConnection.UnregisterEvent(CreateLineAndApplyPullForceOnConnection);
        GameEvents.ThreadEvents.onUpdateLinkMovement.UnregisterEvent(UpdateLink);
        GameEvents.ThreadEvents.onEmptyList_DetectingPoints.UnregisterEvent(ClearDetectedPointsList);

    }


}
