using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public PlushieActiveStitchPart partType;

    public PartConnectedTo partConnectedTo;
    public Vector3 originalRotation;
    public bool moveable = false;
    public bool shouldBeChild = false;
    public bool IsStitched { get; private set; }
    public List<SewPoint> connectPoints;
    public bool head;
    public float pullForce;
    public int totalConnections;
    public int noOfConnections;
    public Transform targetCameraPoint;
    public void MarkStitched()
    {
        IsStitched = true;
    }

    public void ResetStitched()
    {
        IsStitched = false;
    }
}
