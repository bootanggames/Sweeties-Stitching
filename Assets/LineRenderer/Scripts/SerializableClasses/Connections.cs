using System;
using UnityEngine;

[Serializable]
public class Connections 
{
    public Transform point1;
    public Transform point2;

    public Connections(Transform p1, Transform p2)
    {
        this.point1 = p1; this.point2 = p2;   
    }
}
