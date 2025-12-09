using System;
using UnityEngine;

[Serializable]
public class Connections 
{
    public Transform point1;
    public Transform point2;
    public bool isLocked;
    public LineRenderer line;
    //public List<LineRenderer> links;
    public Connections(Transform p1, Transform p2, LineRenderer prefab, float zVal, bool multiple, int stitchCount)
    {
        this.point1 = p1;
        this.point2 = p2;
        isLocked = false;

        this.line = GameObject.Instantiate(prefab);
        Vector3 pos1 = p1.position; pos1.z = zVal;
        Vector3 pos2 = p2.position; pos2.z = zVal;
        this.line.positionCount = 2;
        this.line.SetPosition(0, pos1);
        this.line.SetPosition(1, pos2);
        this.line.material.color = LevelsHandler.instance.currentLevelMeta.levelScriptable.threadColor;

        //multipleLine = multiple;

        //if (multiple)
        //{
        //    SewPoint sp_P1 = p1.GetComponent<SewPoint>();
        //    SewPoint sp_P2 = p2.GetComponent<SewPoint>();

        //    if (sp_P1 == null || sp_P2 == null)
        //    {
        //        Debug.LogWarning("Missing SewPoint component on one or both points.");
        //        return;
        //    }

        //    links = new List<LineRenderer>();
        //    count = stitchCount;
        //    Debug.LogError(" " + count);
        //    if (count <= 0) return;
        //    //int count = Mathf.Min(sp_P1.stitchEffect_ThreadPoints.Count, sp_P2.stitchEffect_ThreadPoints.Count);

        //    for (int i = 0; i < count; i++)
        //    {
        //        Transform threadP1 = sp_P1.stitchEffect_ThreadPoints[i];
        //        Transform threadP2 = sp_P2.stitchEffect_ThreadPoints[i];

        //        if (threadP1 == null || threadP2 == null)
        //            continue;

        //        LineRenderer l = GameObject.Instantiate(prefab);
        //        l.positionCount = 2;

        //        Vector3 pos1 = threadP1.position; pos1.z = zVal;
        //        Vector3 pos2 = threadP2.position; pos2.z = zVal;

        //        l.SetPosition(0, pos1);
        //        l.SetPosition(1, pos2);

        //        l.name = $"link_{i}";
        //        if(!links.Contains(l) )
        //            links.Add(l);
        //    }
        //}
        //else
        //{
        //    this.line = GameObject.Instantiate(prefab);
        //    Vector3 pos1 = p1.position; pos1.z = zVal;
        //    Vector3 pos2 = p2.position; pos2.z = zVal;
        //    this.line.positionCount = 2;
        //    this.line.SetPosition(0, pos1);
        //    this.line.SetPosition(1, pos2);
        //    this.line.material.color = LevelsHandler.instance.currentLevelMeta.threadColor;

        //}
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

    public void UpdateLine(float zVal, bool multiple)
    {
        if (line == null) return;
        Vector3 pos1 = point1.position; pos1.z = zVal;
        Vector3 pos2 = point2.position; pos2.z = zVal;
        line.SetPosition(0, pos1);
        line.SetPosition(1, pos2);
        //if (multiple)
        //{

        //    SewPoint sp_P1 = point1.GetComponent<SewPoint>();
        //    SewPoint sp_P2 = point2.GetComponent<SewPoint>();
        //    if (sp_P1 == null || sp_P2 == null) return;

        //    int count = Mathf.Min(
        //        sp_P1.stitchEffect_ThreadPoints.Count,
        //        sp_P2.stitchEffect_ThreadPoints.Count,
        //        links.Count
        //    );

        //    //Debug.LogError($"[UpdateLine] multiple={multiple}, count={count}, links={links.Count}");

        //    for (int i = 0; i < count; i++)
        //    {
        //        if (links[i] == null) continue;

        //        Vector3 pos1 = sp_P1.stitchEffect_ThreadPoints[i].position;
        //        Vector3 pos2 = sp_P2.stitchEffect_ThreadPoints[i].position;

        //        pos1.z = pos2.z = zVal;
        //        //Debug.LogError(" " + pos1 + " " + pos2);
        //        links[i].SetPosition(0, pos1);
        //        links[i].SetPosition(1, pos2);
        //    }
        //}
        //else
        //{
        //    if (line == null) return;
        //    Vector3 pos1 = point1.position; pos1.z = zVal;
        //    Vector3 pos2 = point2.position; pos2.z = zVal;
        //    line.SetPosition(0, pos1);
        //    line.SetPosition(1, pos2);
        //}

    }

    
}
