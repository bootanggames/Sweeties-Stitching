using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linker : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform[] attachPoints;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float rSpeed;


    [Header(" Test ")]
    [SerializeField] private Link[] links;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            AttachLinks();
                //AttachObjects();
    }

    private void AttachLinks()
    {
        for (int i = 0; i < links.Length; i++)
            AttachObjects(links[i].attach0, links[i].attach1);        
    }

    /*
    private void AttachObjects()
    {
        // Move one object first
        Transform cube = attachPoints[0].parent.parent;

        Vector3 dCubeAttach = (attachPoints[0].position - cube.position).normalized;
        Vector3 dAttach0Attach1 = (attachPoints[1].position - attachPoints[0].position).normalized;

        float rotationSpeed = 1 - Mathf.Abs(Vector3.Dot(dCubeAttach, dAttach0Attach1));
        //Debug.Log("Rotation speed : " + rotationSpeed);

        cube.RotateAround(attachPoints[0].position, Vector3.forward, rotationSpeed * Time.deltaTime * rSpeed);

        Debug.Log("Move speed : " + (1 - Mathf.Abs(rotationSpeed)));

        cube.position += (attachPoints[1].position - attachPoints[0].position) * moveSpeed * Time.deltaTime * (1 - rotationSpeed);

        // Move one object first
        Transform cube2 = attachPoints[1].parent.parent;

        Vector3 dCubeAttach2 = (attachPoints[1].position - cube2.position).normalized;
        Vector3 dAttach1Attach0 = (attachPoints[0].position - attachPoints[1].position).normalized;

        float rotationSpeed2 = 1 - Vector3.Dot(dCubeAttach2, dAttach1Attach0);
        //Debug.Log("Rotation speed : " + rotationSpeed2);

        cube2.RotateAround(attachPoints[1].position, Vector3.forward, rotationSpeed2 * Time.deltaTime * rSpeed);
    }
    */

    private void AttachObjects()
    {
        AttachObjects(attachPoints[0], attachPoints[1]);
        //AttachObjects(attachPoints[1], attachPoints[0]);
        

        AttachObjects(attachPoints[1], attachPoints[0], attachPoints[2]);

        AttachObjects(attachPoints[2], attachPoints[1]);
        
        /*
        AttachObjects(attachPoints[2], attachPoints[0]);
        AttachObjects(attachPoints[0], attachPoints[2]);        
        */
    }

    /*
    private void AttachObjects(Transform attach0, Transform attach1)
    {
        // Maybe work with barycenter ?
        // Like one attach can be linked to multiple attaches but takes the barycenter of other attaches to target

        Transform cube = attach0.parent.parent;

        Vector3 dCubeAttach = (attach0.position - cube.position).normalized;
        Vector3 dAttach0Attach1U = (attach1.position - attach0.position);
        Vector3 dAttach0Attach1 = dAttach0Attach1U.normalized;

        float rotationSpeed = 1 - Vector3.Dot(dCubeAttach, dAttach0Attach1);
        float moveSpeed = Mathf.Abs(Vector3.Dot(dCubeAttach, dAttach0Attach1));

        //if(dAttach0Attach1U.magnitude > .01f)
            cube.RotateAround(attach0.position, Vector3.forward, rotationSpeed * Time.deltaTime * rSpeed);

        cube.position += (attach1.position - attach0.position) * maxMoveSpeed * Time.deltaTime * moveSpeed;
    }
    */

    private void AttachObjects(Transform attach0, params Transform[] attachs)
    {
        Vector3 barycenter = Vector3.zero;

        
        List<Transform> attachsNoUnderSameParent = new List<Transform>();

        for (int i = 0; i < attachs.Length; i++)
        {
            if (attachs[i].parent.parent == attach0.parent.parent)
                continue;

            attachsNoUnderSameParent.Add(attachs[i]);
        }
        
        for (int i = 0; i < attachsNoUnderSameParent.Count; i++)
            barycenter += attachsNoUnderSameParent[i].position;

        barycenter /= attachsNoUnderSameParent.Count;

        AttachObjects(attach0, barycenter);
    }

    private void AttachObjects(Transform attach0, Vector3 barycenter)
    {
        // Maybe work with barycenter ?
        // Like one attach can be linked to multiple attaches but takes the barycenter of other attaches to target

        Transform cube = attach0.parent.parent;

        Vector3 dCubeAttach = (attach0.position - cube.position).normalized;
        Vector3 dAttach0Attach1U = (barycenter - attach0.position);
        Vector3 dAttach0Attach1 = dAttach0Attach1U.normalized;


        float angleFactor = Vector3.Angle(dCubeAttach, dAttach0Attach1) / 180;
        /*
        if (angleFactor < .1f)
            angleFactor = 0;
        */
        Debug.Log(angleFactor);

        //float rotationSpeed = angleFactor;//Vector3.Dot(dCubeAttach, dAttach0Attach1);
        float rotationSpeed = angleFactor * -Mathf.Sign(Vector3.Dot(dCubeAttach, dAttach0Attach1));
        float moveSpeed = 1 - angleFactor;//Mathf.Abs(Vector3.Dot(dCubeAttach, dAttach0Attach1));

        //if (dAttach0Attach1U.magnitude > .1f)
            cube.RotateAround(attach0.position, Vector3.forward, rotationSpeed * Time.deltaTime * rSpeed);

        cube.position += (barycenter - attach0.position) * maxMoveSpeed * Time.deltaTime * moveSpeed;
    }


}


