using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObjectInfo : MonoBehaviour
{
    public Vector3 originalRotation;
    public bool moveable = false;
    public bool shouldBeChild = false;
    public bool IsStitched { get; private set; }
    public List<SewPoint> connectPoints;
    public List<PointPairContainer> pointPair;
    public bool head;
    public float pullForce;
    public void MarkStitched()
    {
        IsStitched = true;
    }

    public void ResetStitched()
    {
        IsStitched = false;
    }
}
