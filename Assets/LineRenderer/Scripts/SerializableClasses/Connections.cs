using Obi;
using System;
using UnityEngine;

[Serializable]
public class Connections 
{
    public Transform point1;
    public Transform point2;
    public bool isLocked;
    public LineRenderer line;
    public bool isMerged = false;
    public Transform originalParentPoint2;
    public float originalDistance;
    public Connections(Transform p1, Transform p2, LineRenderer prefab, float zVal)
    {
        this.point1 = p1; this.point2 = p2;
        isLocked = false;
        
        this.line = GameObject.Instantiate(prefab);
        Vector3 pos1 = p1.position; pos1.z = zVal;
        Vector3 pos2 = p2.position; pos2.z = zVal;
        this.line.positionCount = 2;
        this.line.SetPosition(0, pos1);
        this.line.SetPosition(1, pos2);
        this.line.name = "link";
        originalDistance = Vector3.Distance(p1.position, p2.position);
    }
    public void DestroyPreviousLine()
    {
        if(this.line != null)
        {
            GameObject.Destroy(this.line.gameObject);
        }
    }
    public bool AlreadyHaveConnection(Connections other)
    {
        if (other == null) return false;

        return (point1 == other.point1 && point2 == other.point2) ||
               (point1 == other.point2 && point2 == other.point1);
    }

    public override bool Equals(object obj) => AlreadyHaveConnection(obj as Connections);

    public override int GetHashCode()
    {
        int h1 = point1 != null ? point1.GetHashCode() : 0;
        int h2 = point2 != null ? point2.GetHashCode() : 0;
        return h1 ^ h2;
    }

    public void UpdateLine(float zVal)
    {
        if (line == null) return;
        Vector3 pos1 = point1.position; pos1.z = zVal;
        Vector3 pos2 = point2.position; pos2.z = zVal;
        line.SetPosition(0, pos1);
        line.SetPosition(1, pos2);
    }
}
