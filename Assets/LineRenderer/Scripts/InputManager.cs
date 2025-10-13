using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector3 firstTouch;
    Vector3 dragTouchValue;
    [SerializeField]bool drag = false;

    float moveX = 0;
    float moveY = 0;

    [SerializeField] Vector2 moveLimit;
    [SerializeField]Vector2 lastNeedlePos;
    [SerializeField] Transform startPoint;
    [SerializeField] float needleOffset;
    
    private void Update()
    {
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler == null) return;
        if (!gameHandler.gameStates.Equals(GameStates.Gamestart)) return;
        GetInput();
    }
    void GetInput()
    {
        if (Input.GetMouseButtonDown(0) && !drag)
        {
            drag = true;
            firstTouch = CalculateCurrentPosition();
            firstTouch = startPoint.position;
            GameEvents.ThreadEvents.onInitialiseRope.RaiseEvent(firstTouch);

        }
        else if (Input.GetMouseButton(0) && drag)
        {
          
            dragTouchValue = CalculateCurrentPosition();

            float reference = Mathf.Min(Screen.width, Screen.height);

            float normalizedX = (dragTouchValue.x - firstTouch.x) / reference;
            float normalizedY = (dragTouchValue.y - firstTouch.y) / reference;

            moveX = lastNeedlePos.x + normalizedX * moveLimit.x;
            moveY = lastNeedlePos.y + normalizedY * moveLimit.y;

            moveX = Mathf.Clamp(moveX, -moveLimit.x, moveLimit.x);
            moveY = Mathf.Clamp(moveY, -moveLimit.y, moveLimit.y);

            //Vector2 newPos = new Vector2 (dragTouchValue.x, (dragTouchValue.y + needleOffset));//---ADDED NEW LINE  
            GameEvents.ThreadEvents.onAddingPositionToRope.RaiseEvent(new Vector2(moveX, moveY));
        }
        else if (Input.GetMouseButtonUp(0) && drag)
        {
            drag = false;
            lastNeedlePos = new Vector2(moveX, moveY);
            GameEvents.PointConnectionHandlerEvents.onStopTweens.RaiseEvent();
            GameEvents.ThreadEvents.setThreadInput.RaiseEvent(true);

        }

    }

    Vector3 CalculateCurrentPosition()
    {
        Vector3 position;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float requiredWidth = screenWidth / 2;
        float requiredHeight = screenHeight / 2;
        float moveXRange = Input.mousePosition.x - requiredWidth;
        float moveYRange = Input.mousePosition.y - requiredHeight;
        position = new Vector3(moveXRange, moveYRange, Input.mousePosition.z);
        //position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return position;
    }
}
