using System.Collections.Generic;
using Unity.Splines.Examples;
using UnityEngine;
using UnityEngine.Splines;

public class ThreadManager : MonoBehaviour,IThreadManager
{
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] Vector3 currentRopeStartPosition;
    Vector2 prevMouseDragPosition;
    Vector2 mousePositionWhenAttachedToHole;
    SewPoint lastSewPoint;

    [SerializeField] float minDistanceBetweenSewPoints;
    [SerializeField] int threadMaxLength;

    [SerializeField] private Transform threadParent;
    int attachTouched;
    List<Transform> attachedTransforms = new List<Transform>();

    LineRenderer instantiatedLine;
   
    private void OnEnable()
    {
        if(instantiatedLine == null)
        {
            instantiatedLine = Instantiate(lineRenderer, this.transform.position, Quaternion.identity);
            instantiatedLine.transform.SetParent(this.transform);
            instantiatedLine.transform.position = Vector3.zero;
        }

        RegisterService();
        instantiatedLine.positionCount = threadMaxLength;

    }
    private void OnDisable()
    {
        UnRegisterService();
    }
 
    public void AddFirstPositionOnMouseDown(Vector2 headPos)
    {
        instantiatedLine.SetPosition(0, headPos);
        currentRopeStartPosition = instantiatedLine.GetPosition(0);
    }

  
    public void AddPositionToLineOnDrag(Vector2 mousePos)
    {
        Vector3 targetRopePosition = new Vector3(mousePos.x, mousePos.y, 0);

        Vector3 direction = targetRopePosition - currentRopeStartPosition;
        float threadLength = threadMaxLength * minDistanceBetweenSewPoints;

        if (direction.magnitude > threadLength)
            targetRopePosition = currentRopeStartPosition + direction.normalized * threadLength;

        instantiatedLine.SetPosition(0, targetRopePosition);

        MoveThread(instantiatedLine, false);

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
   
    void CreateLineAndApplyPullForceOnConnection()
    {
        //GetAttachedPointsToCreateLink --call this function
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.RegisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.RegisterEvent(AddPositionToLineOnDrag);

    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.UnregisterEvent(AddFirstPositionOnMouseDown);
        GameEvents.ThreadEvents.onAddingPositionToRope.UnregisterEvent(AddPositionToLineOnDrag);

    }


}
