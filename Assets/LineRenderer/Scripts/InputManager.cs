using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] float sensitivity;
    [SerializeField] float moveXRange;
    [SerializeField] float minYRange;
    [SerializeField] float maxYRange;
    [SerializeField] Vector2 currentPosition;
    bool pressed = false;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !pressed)
        {
            pressed = true;
        }
        GetInput();
    }
    void GetInput()
    {
        if (Input.GetMouseButton(0) && pressed)
        {
            float xVal = Input.GetAxis("Mouse X");
            float yVal = Input.GetAxis("Mouse Y");
            if(xVal != 0 || yVal != 0)
            {
                currentPosition += new Vector2(xVal, yVal) * sensitivity;

                currentPosition.x = Mathf.Clamp(currentPosition.x, -moveXRange, moveXRange);
                currentPosition.y = Mathf.Clamp(currentPosition.y, minYRange, maxYRange);

                Vector3 newPosition = GameEvents.NeedleEvents.OnGettingCurrentPositionFromFixedStart.Raise(currentPosition);
                GameEvents.NeedleEvents.OnNeedleMovement.RaiseEvent(newPosition);
                GameEvents.ThreadEvents.onAddingPositionToRope.RaiseEvent(newPosition);

            }

        }
    }
}
