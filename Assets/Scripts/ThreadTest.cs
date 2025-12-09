using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThreadTest : MonoBehaviour
{
    Vector3 dragTouchValue;
    [SerializeField] bool drag = false;
    [SerializeField] float needleOffset;
    public LineRenderer threadLine;
    [SerializeField] List<Vector3> positions;
    [SerializeField] float zVal;
    [SerializeField] Transform startPoint;
    [SerializeField] float minDistance;
    [SerializeField] List<Vector3> smallestPos;

    private void Update()
    {
        GetInput();
    }
    [SerializeField]int step = 1; // more = smoother curve

    void GetInput()
    {
        if (Input.GetMouseButtonDown(0) && !drag)
        {
            drag = true;
        }
        else if (Input.GetMouseButton(0) && drag)
        {
            Vector3 fixedStart = startPoint.position;
            fixedStart.z = zVal;

            // Convert mouse to world position
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z - zVal);
            Vector3 dragPos = Camera.main.ScreenToWorldPoint(mousePos);
            dragPos = new Vector3(dragPos.x, dragPos.y + needleOffset, zVal);

            // Initialize positions with fixed start
            if (positions.Count == 0)
                positions.Add(fixedStart);

            // Only add drag point if far enough
            if (Vector3.Distance(positions[positions.Count - 1], dragPos) > minDistance)
                positions.Add(dragPos);

            List<Vector3> linePoints = new List<Vector3>();
            linePoints.Add(fixedStart);

            // Dynamic first segment curve
            if (positions.Count >= 2)
            {
                Vector3 first = fixedStart;
                Vector3 second = positions[1];

                int steps = 10;
                float amplitude = Vector3.Distance(first, second) * 0.1f; // amplitude proportional to length
                float frequency = 1f; // low frequency for gentle curve

                for (int i = 0; i <= steps; i++)
                {
                    float t = i / (float)steps;
                    Vector3 point = Vector3.Lerp(first, second, t);

                    // Compute a perpendicular direction dynamically
                    Vector3 dir = (second - first).normalized;
                    Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;

                    // Offset perpendicular dynamically based on t
                    point += perp * Mathf.Sin(t * Mathf.PI * frequency) * amplitude;

                    linePoints.Add(point);
                }

                // Add remaining points normally
                for (int i = 2; i < positions.Count; i++)
                    linePoints.Add(positions[i]);
            }
            else if (positions.Count == 1)
            {
                linePoints.Add(dragPos);
            }

            // Update LineRenderer
            threadLine.positionCount = linePoints.Count;
            threadLine.SetPositions(linePoints.ToArray());
        }
        else if (Input.GetMouseButtonUp(0) && drag)
        {
            drag = false;
        }

    }
}
