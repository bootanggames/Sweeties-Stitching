using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidLinker : MonoBehaviour
{
    [SerializeField] private Link[] links;
    [SerializeField] private float forceMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.Space))
            return;

        for (int i = 0; i < links.Length; i++)
            ManageLink(links[i]);
    }
    
    private void ManageLink(Link link)
    {
        if(link.attach0.parent == link.attach1.parent)
            return;

        Rigidbody parentRigidbody = link.attach0.GetComponentInParent<Rigidbody>();

        Vector3 force = (link.attach1.position - link.attach0.position);
        force.z = 0;

        parentRigidbody.AddForceAtPosition(force, link.attach0.position);
    }
}
