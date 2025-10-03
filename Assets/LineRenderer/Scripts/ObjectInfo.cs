using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public Vector3 originalRotation;

    public bool IsStitched { get; private set; }

    public void MarkStitched()
    {
        IsStitched = true;
    }

    public void ResetStitched()
    {
        IsStitched = false;
    }
}
