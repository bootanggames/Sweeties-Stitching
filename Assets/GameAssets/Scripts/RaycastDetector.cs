using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Splines.Examples;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class RaycastDetector : MonoBehaviour, IRaycastDetector
{
    [SerializeField] float distance;
    [SerializeField]LayerMask layerMask;
    [SerializeField] bool locked = false;
    [SerializeField] Vector3 needleStartPoint;
    [SerializeField] SplineRenderer splineRenderer;
    [SerializeField]
    Mesh m_SampleDot;

    [SerializeField]
    Material m_SampleMat, m_ControlPointMat;
    [SerializeField] int totalHoles;
    [SerializeField] Animator headAnim;
    [field: SerializeField] public bool isInTrigger { get; private set; }
    [field: SerializeField] public bool stopLine { get; private set; }

    public List<GameObject> holes;
    [SerializeField] GameObject ropeSolver;

    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [SerializeField] float minDistanceBetweenStartAndEndPoint;
    [SerializeField] float _distance;
    [SerializeField] float speed;
    void MoveEndPoint()
    {
        _distance = Vector3.Distance(startPoint.position, endPoint.position);
        if (_distance < minDistanceBetweenStartAndEndPoint)
        {
            endPoint.DOMove(startPoint.position, speed).SetEase(Ease.Linear).OnComplete(() =>
            {

            });
        }
    }
    public void GetPoints(GameObject p)
    {
        if (!holes.Contains(p))
            holes.Add(p);

        Invoke(nameof(EnableAniamtion), 0.5f);

    }

    public void EnableAniamtion()
    {
        Debug.LogError(" " + holes.Count);
        if (holes.Count >= totalHoles)
        {
            Invoke(nameof(DisableRope), 0.5f);
        }
        CancelInvoke(nameof(EnableAniamtion));
    }
    void DisableRope()
    {
        ropeSolver.SetActive(false);
        //headAnim.enabled = true;

        CancelInvoke(nameof(DisableRope));
    }
    private void OnEnable()
    {
        GameEvents.RaycastDetectorEvents.onResetNeedle.RegisterEvent(ResetNeedle);
        GameEvents.RaycastDetectorEvents.onUpdateNeedle.RegisterEvent(UpdateNeedle);
        RegisterService();
    }

    private void OnDisable()
    {
        GameEvents.RaycastDetectorEvents.onResetNeedle.UnregisterEvent(ResetNeedle);
        GameEvents.RaycastDetectorEvents.onUpdateNeedle.UnregisterEvent(UpdateNeedle);
        UnRegisterService();
    }
    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            //GameEvents.ThreadManagerEvents.onLineReset.RaiseEvent();
            if (needle)
            {
                needle.transform.position = needleStartPoint;

                GameEvents.SewingThreadSplineEvents.onResetSpline.RaiseEvent();
                ResetNeedle();
            }
        }
        if (Input.GetMouseButton(0))
        {
            RaycastPoints(Input.mousePosition);
        }
    }
    RaycastHit hit;
    [SerializeField] GameObject needle = null;
    [SerializeField] List<LineRenderer> stiches = new List<LineRenderer>();
    [SerializeField] List<Vector3> points = new List<Vector3>();
    private void RaycastPoints(Vector3 touchPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPoint);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
        if (Physics.Raycast(ray.origin, ray.direction,out hit, distance, layerMask))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("needle") && !locked)
                {
                    locked = true;
                    needle = hit.collider.gameObject;
                }
              
                if (needle)
                {
                    Vector3 pos = hit.point;
                    pos.z = -1;
                    needle.transform.position = pos;
                    //MoveEndPoint();
                    if (!isInTrigger)
                        GameEvents.SewingThreadSplineEvents.onAppSplineStitchPoints.RaiseEvent(pos);
                    else
                    {
                        GameEvents.SewingThreadSplineEvents.onResetSpline.RaiseEvent();

                        if (stopLine)
                        {
                            if (!points.Contains(hit.point))
                                points.Add(hit.point);
                            Invoke(nameof(CreateStitch), 0.7f);
                        }
                    }
               
                }

            }
        }
      
    }

    void CreateStitch()
    {
        CreateLine(points);
        CancelInvoke(nameof(CreateStitch));
        stopLine = false;

    }
    public Material lineMaterial;
    private List<LineRenderer> dashSegments = new List<LineRenderer>();

    void CreateLine(List<Vector3> positions)
    {
        if (positions.Count == 0) return;
        GameObject line = new GameObject();
        line.AddComponent<LineRenderer>();
        line.transform.SetParent(this.transform);
        LineRenderer l = line.GetComponent<LineRenderer>();
        if (!stiches.Contains(l))
            stiches.Add(l);
        l.numCornerVertices = 90;
        l.numCapVertices = 90;
        l.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        l.widthCurve = new AnimationCurve(new Keyframe(0f, 0.08f));
        l.startColor = Color.lightGray;
        l.endColor = Color.lightGray; 
        l.material = lineMaterial;
        l.useWorldSpace = true;
        l.textureMode = LineTextureMode.Tile;
        l.textureScale = new Vector2(2, 1);
        l.positionCount = positions.Count;
        for (int g = 0; g < positions.Count; g++)
        {
            Vector3 pos = positions[g];
            pos.z = -1;
            l.SetPosition(g, pos);
        }
        points.Clear();

    }
    void ResetNeedle()
    {
        needle.tag = "Untagged";
        needle = null;
        locked = false;
    }

    void UpdateNeedle(GameObject n_obj)
    {
        needle = n_obj;
    }

    public void RegisterService()
    {
        ServiceLocator.RegisterService<IRaycastDetector>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IRaycastDetector>(this);
    }

    public void IsInTrigger(bool val)
    {
        isInTrigger = val;
    }

    public bool GetTriggerVal()
    {
       return isInTrigger;
    }

    public void StopLinerenderer(bool val)
    {
        stopLine = val;
    }

    public bool IsStopped()
    {
        return stopLine;
    }
}
