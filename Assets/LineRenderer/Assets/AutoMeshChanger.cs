using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMeshChanger : MonoBehaviour
{
    [NaughtyAttributes.Button]
    public void SetMesh()
    {
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    [NaughtyAttributes.Button]
    public void ResetPositionAndRotation()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
