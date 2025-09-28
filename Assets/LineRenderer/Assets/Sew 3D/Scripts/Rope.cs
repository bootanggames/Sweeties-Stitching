using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetSystems;

public class Rope : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private LineRenderer ropePrefab;
    [SerializeField] private Transform temporaryRopesParent;
    [SerializeField] private Transform linkRopesParent; 
    private LineRenderer previousRope;
    private LineRenderer currentRope;

    [Header(" Controlling ")]
    private Vector3 clickedRopeHeadPos;
    private Vector2 previousMouseDelta;
    private Vector2 mouseDeltaWhenAttached;
    private AttachPoint lastAttachPoint;

    [Header(" Settings ")]
    [SerializeField] private float minDistanceBetweenPoints;
    private bool canControl = true;

    [Header(" Events ")]
    public static Action<AttachPoint> OnAttachPointTouched;
    public static Action<float> OnRopeMoving;

    // Start is called before the first frame update
    void Start()
    {
        currentRope = Instantiate(ropePrefab, temporaryRopesParent);

        Sewable.OnAutoWeldStarted += SewableCompleteCallback;
        Sewable.OnComplete += SewableCompleteCallback;
    }

    private void OnDestroy()
    {
        Sewable.OnAutoWeldStarted -= SewableCompleteCallback;
        Sewable.OnComplete -= SewableCompleteCallback;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MouseDownCallback()
    {
        if (!canControl)
            return;

        clickedRopeHeadPos = currentRope.GetPosition(0);
    }

    public void MouseDragCallback(Vector2 mouseDelta)
    {
        if (!canControl)
            return;

        Vector3 _clickedRopeHeadPos = clickedRopeHeadPos;

        if (lastAttachPoint != null)
            _clickedRopeHeadPos = lastAttachPoint.transform.position;

        Vector3 targetRopeHeadPosition = clickedRopeHeadPos + new Vector3(mouseDelta.x - mouseDeltaWhenAttached.x, mouseDelta.y - mouseDeltaWhenAttached.y, 0);

        // If we touched an attach point, clamp the length of the rope
        if(lastAttachPoint != null)
        {
            Vector3 uDirection = targetRopeHeadPosition - _clickedRopeHeadPos;
            float maxRopeLength = 49 * minDistanceBetweenPoints;

            if (uDirection.magnitude >= maxRopeLength)
                targetRopeHeadPosition = _clickedRopeHeadPos + uDirection.normalized * maxRopeLength;
        }
        
        currentRope.SetPosition(0, targetRopeHeadPosition);


        float moveMagnitude = (mouseDelta - previousMouseDelta).magnitude;

        if (moveMagnitude > 0)
        {
            MoveRope(currentRope);
            OnRopeMoving?.Invoke(moveMagnitude);
            UpdateLinkRopes();
        }

        if (previousRope != null)
            RetractPreviousRope(moveMagnitude);

        previousMouseDelta = mouseDelta;
    }

    public void MouseUpCallback()
    {
        if (!canControl)
            return;

        mouseDeltaWhenAttached = Vector2.zero;
    }

    private void RetractPreviousRope(float magnitude)
    {
        if(magnitude > 0)
            MoveRope(previousRope, true);        
    }

    private void MoveRope(LineRenderer rope, bool isPreviousRope = false)
    {
        for (int i = 1; i < rope.positionCount; i++)
        {
            Vector3 previousPointPosition = rope.GetPosition(i - 1);
            Vector3 uDirection = rope.GetPosition(i) - previousPointPosition;

            float minDistance = isPreviousRope ? 0 : minDistanceBetweenPoints;
            float lerp = isPreviousRope ? .3f : 1f;

            Vector3 targetPosition = previousPointPosition + uDirection.normalized * minDistance;

            Vector3 lerpedPosition = Vector3.Lerp(rope.GetPosition(i), targetPosition, lerp);
            
            rope.SetPosition(i, lerpedPosition);
        }

        if(!isPreviousRope && lastAttachPoint != null)
        {
            rope.SetPosition(rope.positionCount - 1, lastAttachPoint.transform.position);

            for (int i = rope.positionCount - 2; i >= 0; i--)
            {
                Vector3 nextPointPosition = rope.GetPosition(i + 1);
                Vector3 uDirection = rope.GetPosition(i) - nextPointPosition;

                Vector3 targetPosition = nextPointPosition + uDirection.normalized * minDistanceBetweenPoints;

                Vector3 lerpedPosition = Vector3.Lerp(rope.GetPosition(i), targetPosition, 1f);
                
                rope.SetPosition(i, lerpedPosition);

            }
        }
    }

    int attachTouched;
    List<Transform> attachsTransforms = new List<Transform>();
    public void AttachTo(AttachPoint attachPoint)
    {
        attachsTransforms.Add(attachPoint.transform);

        attachTouched++;

        if(attachTouched > 1)
        {
            temporaryRopesParent.Clear();

            Instantiate(ropePrefab, linkRopesParent);
        }
        else
        {
            currentRope.SetPosition(0, attachPoint.transform.position);
            previousRope = currentRope;
        }

        currentRope = Instantiate(ropePrefab, attachPoint.transform.position, Quaternion.identity, temporaryRopesParent);

        OnAttachPointTouched?.Invoke(attachPoint);

        lastAttachPoint = attachPoint;
        clickedRopeHeadPos = attachPoint.transform.position;

        mouseDeltaWhenAttached = previousMouseDelta;


        //currentRope = Instantiate(ropePrefab, attachPoint.transform.position, Quaternion.identity, temporaryRopesParent);

        currentRope.name = "Rope " + transform.childCount;

        for (int i = 0; i < currentRope.positionCount; i++)
            currentRope.SetPosition(i, attachPoint.transform.position);
        
    }

    private void UpdateLinkRopes()
    {
        for (int i = 0; i < linkRopesParent.childCount; i++)
        {
            LineRenderer line = linkRopesParent.GetChild(i).GetComponent<LineRenderer>();

            line.positionCount = 2;
            line.SetPosition(0, attachsTransforms[i].position);
            line.SetPosition(1, attachsTransforms[i + 1].position);
        }
    }

    public Vector3 GetHeadPosition()
    {
        return currentRope.GetPosition(0);
    }

    private void SewableCompleteCallback()
    {
        PreventControl();

        currentRope.enabled = false;

        for (int i = 0; i < linkRopesParent.childCount; i++)
            linkRopesParent.GetChild(i).GetComponent<LineRenderer>().enabled = false;
        
    }

    private void PreventControl()
    {
        canControl = false;
    }
}
