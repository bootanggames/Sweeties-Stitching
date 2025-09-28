using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rope))]
public class RopeDetection : MonoBehaviour
{

    [Header(" Settings ")]
    [SerializeField] private LayerMask attachPointsLayer;

    private Rope rope;

    private void Awake()
    {
        rope = GetComponent<Rope>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectAttachPoints();
    }

    private void DetectAttachPoints()
    {
        Collider[] detectedAttachPoints = Physics.OverlapSphere(rope.GetHeadPosition(), .05f, attachPointsLayer);

        if (detectedAttachPoints.Length <= 0)
            return;

        AttachPoint attachPoint = detectedAttachPoints[0].GetComponent<AttachPoint>();

        if (attachPoint.IsAttached())
            return;

        attachPoint.Attach();

        rope.AttachTo(attachPoint);
    }
}
