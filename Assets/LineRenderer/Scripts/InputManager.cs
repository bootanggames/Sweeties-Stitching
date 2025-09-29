using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Vector3 firstTouch;
    [SerializeField] Vector3 dragTouchValue;
    bool drag = false;
    float moveX = 0;
    float moveY = 0;
    [SerializeField] Vector2 moveLimit;
    private void Update()
    {
        
        GetInput();
    }
    void GetInput()
    {
        if (Input.GetMouseButtonDown(0) && !drag)
        {
            drag = true;
            firstTouch = CalculateCurrentPosition();
            dragTouchValue = CalculateCurrentPosition();
        }
        else if (Input.GetMouseButton(0) && drag)
        {
            dragTouchValue = CalculateCurrentPosition();
            moveX = dragTouchValue.x - firstTouch.x;
            moveX *= moveLimit.x / (float)Screen.width;

            moveY = dragTouchValue.y - firstTouch.y;
            moveY *= moveLimit.y / (float)Screen.height;

            GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(new Vector2(moveX, moveY));

        }
        else if(Input.GetMouseButtonUp(0) && drag)
        {
            moveX = 0;
            moveY = 0;
            drag = false;
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

        return position;
    }
}
