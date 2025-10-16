using UnityEngine;

public class Test_NeedleInput : MonoBehaviour
{
    bool drag = false;
    public GameObject needleEye;
    Vector3 firstTouch;
    Vector3 dragPos;
    Vector3 lastTouch;
    Vector3 prevDragPos;
    float zVal = 0;

    [SerializeField] Vector3 direction;
    [SerializeField] float rotationSpeed = 360f;
    [SerializeField] float angleOffset;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !drag)
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            firstTouch.z = zVal;
            prevDragPos = firstTouch;
            drag = true;
        }

        if (Input.GetMouseButton(0) && drag)
        {
            dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragPos.z = zVal;
            float sqMagnitude = (dragPos - prevDragPos).sqrMagnitude;
            direction = (dragPos - prevDragPos).normalized;

         
            NeedleRotation(sqMagnitude, direction);
            needleEye.transform.position = dragPos;

            prevDragPos = dragPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastTouch.z = zVal;
            drag = false;
        }
    }

    void NeedleRotation(float magnitude , Vector3 _direction)
    {
        if (magnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - angleOffset); // Adjust -90f based on sprite orientation
            needleEye.transform.rotation = Quaternion.RotateTowards(
                needleEye.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

    }
}
