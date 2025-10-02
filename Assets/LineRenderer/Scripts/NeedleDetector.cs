using UnityEngine;

public class NeedleDetector : MonoBehaviour
{
    [SerializeField]Transform needleTransform;
    [SerializeField] LayerMask detectLayer;
    [SerializeField] float detectionRadius;
    void Update()
    {
        DetectPoints();
    }
    void DetectPoints()
    {
        Collider[] colliders = Physics.OverlapSphere(needleTransform.position, detectionRadius, detectLayer);
        if (colliders.Length <= 0) return;
        SewPoint sewPoint = colliders[0].GetComponent<SewPoint>();
        if (sewPoint.IsSelected()) return;
        sewPoint.Selected();
        sewPoint.GetComponent<MeshRenderer>().enabled = false;
        GameEvents.ThreadEvents.onCreatingConnection.RaiseEvent(sewPoint);
    }

    private void OnDrawGizmos()
    {
        if (needleTransform == null) return;

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(needleTransform.position, detectionRadius);
    }

}
