using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTest : MonoBehaviour
{
    [Header(" Force Pos ")]
    [SerializeField] private Transform forceTransform;
    [SerializeField] private Transform target;
    [SerializeField] private float x;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 force = (target.position - forceTransform.position) * x;
        GetComponent<Rigidbody>().AddForceAtPosition(force, forceTransform.position, ForceMode.Force);
    }
}
