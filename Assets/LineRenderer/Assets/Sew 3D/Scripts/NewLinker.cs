using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLinker : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private List<Link> links;
    [SerializeField] private float forceMultiplier;
    private float actualForceMultiplier;
    [SerializeField] private float torqueMultiplier;

    private void Awake()
    {
        Rope.OnAttachPointTouched += AttachPointTouchedCallback;
        Rope.OnRopeMoving += RopeMovingCallback;
    }

    private void OnDestroy()
    {
        Rope.OnAttachPointTouched -= AttachPointTouchedCallback;
        Rope.OnRopeMoving -= RopeMovingCallback;
    }

    List<AttachPoint> attachPoints = new List<AttachPoint>();
    /*
    private void AttachPointTouchedCallback(AttachPoint attachPoint)
    {
        attachPoints.Add(attachPoint);

        if (attachPoints.Count < 2)
            return;

        CreateNewLink(attachPoints[attachPoints.Count - 2], attachPoints[attachPoints.Count - 1]);
    }
    */
    
    private void AttachPointTouchedCallback(AttachPoint attachPoint)
    {
        Taptic.Light();

        attachPoints.Add(attachPoint);

        if (attachPoints.Count % 2 != 0)
            return;        

        CreateNewLink(attachPoints[attachPoints.Count - 2], attachPoints[attachPoints.Count - 1]);
    }

    private void CreateNewLink(AttachPoint attachPoint)
    {
        Link link = new Link(attachPoint.transform);
        links.Add(link);
    }

    private void CreateNewLink(AttachPoint attachPoint0, AttachPoint attachPoint1)
    {
        Link link = new Link(attachPoint0.transform, attachPoint1.transform);
        links.Add(link);
    }

    private void RopeMovingCallback(float moveMagnitude)
    {
        actualForceMultiplier = forceMultiplier * moveMagnitude;
        ApplyForcesToLinks();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            ApplyForcesToLinks();
    }


    public void ApplyForcesToLinks()
    {
        for (int i = 0; i < links.Count; i++)
            ManageLink(links[i]);       
    }

    private void ManageLink(Link link)
    {
        //if(link.enabled)
        ApplyForces(link.attach0, link.attach1);
        ApplyForces(link.attach1, link.attach0);
    }

    private void ApplyForces(Transform p0, Transform p1)
    {
        if (p1 == null)
            return;

        if (p0.parent == p1.parent)
            return;

        Transform cube = p0.parent.parent;
        Transform otherCube = p1.parent.parent;

        Vector3 f = (p1.position - p0.position);

        if (f.magnitude < .3f)
        {
            f = f.normalized * .3f;
        }

        Vector3 torque;
        Vector3 force = SetForceAtPosition(f, p0.position, cube.position, out torque);

        Vector3 targetPos = cube.position + (force * Time.deltaTime * actualForceMultiplier);

        cube.position = targetPos;

        if (Mathf.Abs(torque.z) < .3f)
            torque.z = Mathf.Sign(torque.z) * .3f;

        torque = Vector3.forward * torque.z;

        cube.Rotate(torque * torqueMultiplier * Time.deltaTime);

        Vector3 angles = cube.eulerAngles;
        angles.x = 0;
        angles.y = 0;
        cube.eulerAngles = angles;
        
        Vector3 otherAngles = otherCube.eulerAngles;
        otherAngles.x = 0;
        otherAngles.y = 0;
        otherCube.eulerAngles = otherAngles;
    }

    private Vector3 SetForceAtPosition(Vector3 f, Vector3 point, Vector3 centerOfMass, out Vector3 torque)
    {
        torque = Vector3.Cross(point - centerOfMass, f);
        return f;
    }
}

[System.Serializable]
public class Link
{
    //public bool enabled;
    public Transform attach0;
    public Transform attach1;

    public Link(Transform attach0)
    {
        this.attach0 = attach0;
    }

    public Link(Transform attach0, Transform attach1)
    {
        this.attach0 = attach0;
        this.attach1 = attach1;
    }
}