using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    Vector3 firstTouch;
    Vector3 dragTouchValue;
    [SerializeField]bool drag = false;

    [SerializeField] Vector2 moveLimit;
    [SerializeField]Vector2 lastNeedlePos;
    [SerializeField] Transform startPoint;
    [SerializeField] float needleOffset;
    Vector3 prevDragPos;
    [SerializeField] Vector3 direction;
    [SerializeField] GraphicRaycaster canvasGraphicRaycaster;
    [SerializeField] EventSystem eventSystem;
    private void Start()
    {
        Input.multiTouchEnabled = false;
    }
    private void Update()
    {
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler == null) return;
        if (!gameHandler.gameStates.Equals(GameStates.Gamestart)) return;
        else
            if (IsPointerOverUIElement()) return;

        GetInput();
    }
    void GetInput()
    {
        if (Input.GetMouseButtonDown(0) && !drag)
        {

            drag = true;
            firstTouch = CalculateCurrentPosition();
            GameEvents.ThreadEvents.onInitialiseRope.RaiseEvent(firstTouch);
            Invoke("EnableDetection", 0.15f);
            //Debug.LogError("down");
        }
        else if (Input.GetMouseButton(0) && drag)
        {

            dragTouchValue = CalculateCurrentPosition();

            Vector2 newPos = new Vector2(dragTouchValue.x, (dragTouchValue.y + needleOffset));
            GameEvents.ThreadEvents.onAddingPositionToRope.RaiseEvent(newPos);
            //------Needle Rotation--------
            direction = (dragTouchValue - prevDragPos);
            Vector3 normalisedDirection = direction.normalized;
            float magnitude = direction.sqrMagnitude;
            GameEvents.NeedleEvents.onNeedleRotation.RaiseEvent(magnitude, normalisedDirection);
            //-----------------------------

            prevDragPos = dragTouchValue;
        }
        else if (Input.GetMouseButtonUp(0) && drag)
        {
            drag = false;
            //GameEvents.PointConnectionHandlerEvents.onStopTweens.RaiseEvent();
            GameEvents.ThreadEvents.setThreadInput.RaiseEvent(true);

        }

    }
    public bool IsPointerOverUIElement()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        canvasGraphicRaycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }
    Vector3 CalculateCurrentPosition()
    {
        Vector3 position;
        position = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(position);
    }

    void EnableDetection()
    {
        var pointDetector = ServiceLocator.GetService<INeedleDetector>();

        if (pointDetector != null)
        {
            if (!pointDetector.detect)
                pointDetector.detect = true;
        }
        CancelInvoke("EnableDetection");
    }
}
