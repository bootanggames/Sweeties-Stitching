using System.Collections.Generic;
using UnityEngine;

public class NeedleDetector : MonoBehaviour, INeedleDetector
{
    [SerializeField]Transform needleTransform;
    [SerializeField] LayerMask detectLayer;
    [field: SerializeField] public float detectionRadius {  get; private set; }
    [field: SerializeField] public float minDetectionRadius { get; private set; }
    [field: SerializeField] public float maxDetectionRadius { get; private set; }
    [field: SerializeField] public bool detect { get; set; }
    [field: SerializeField] public List<SewPoint> pointsDetected {  get; private set; }
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    //void Update()
    //{
    //    DetectPoints();
    //}
    void SetRadiusValue(float val)
    {
        detectionRadius = val;
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("StitchPoint"))
        {
            if (!detect) return;
            SewPoint sewPoint = other.GetComponent<SewPoint>();
            if (sewPoint.IsSelected()) return;
            if (sewPoint.connected) return;
            sewPoint.Selected(true);

            sewPoint.GetComponent<Collider>().enabled = false;
            sewPoint.name = sewPoint.transform.parent.name + "_sew_" + sewPoint.name;
            PlaySound();
            sewPoint.ChangeTextColor(Color.green);
            GameEvents.EffectHandlerEvents.onSelectionEffect.RaiseEvent(sewPoint.transform);
            GameEvents.ThreadEvents.onCreatingConnection.RaiseEvent(sewPoint);

            if (!pointsDetected.Contains(sewPoint))
                pointsDetected.Add(sewPoint);
            var pointsHandler = ServiceLocator.GetService<IPointConnectionHandler>();

            if (pointsHandler != null)
            {
                if (pointsDetected.Count > 0)
                {
                    if (pointsDetected.Count % 2 == 0)
                    {
                        var threadHandler = ServiceLocator.GetService<IThreadManager>();
                        if (threadHandler != null)
                            threadHandler.ScaleDownAllPoints();
                    }
                }
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
    void DetectPoints()
    {
        if (!detect) return;
        Collider[] colliders = Physics.OverlapSphere(needleTransform.position, detectionRadius, detectLayer);
        if (colliders.Length <= 0) return;
        SewPoint sewPoint = colliders[0].GetComponent<SewPoint>();
        if (sewPoint.IsSelected()) return;
        if (sewPoint.connected) return;
      

        sewPoint.Selected(true);

        sewPoint.GetComponent<Collider>().enabled = false;
        sewPoint.name = sewPoint.transform.parent.name+"_sew_"+ sewPoint.name;
        PlaySound();
        sewPoint.ChangeTextColor(Color.green);
        GameEvents.EffectHandlerEvents.onSelectionEffect.RaiseEvent(sewPoint.transform);
        GameEvents.ThreadEvents.onCreatingConnection.RaiseEvent(sewPoint);

        if (!pointsDetected.Contains(sewPoint))
            pointsDetected.Add(sewPoint);
        var pointsHandler = ServiceLocator.GetService<IPointConnectionHandler>();

        if (pointsHandler != null)
        {
            if (pointsDetected.Count > 0)
            {
                if (pointsDetected.Count % 2 == 0)
                {
                    var threadHandler = ServiceLocator.GetService<IThreadManager>();
                    if (threadHandler != null)
                        threadHandler.ScaleDownAllPoints();
                }
            }
        }

    }
    
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.sewing;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
        HepticManager.instance.HapticEffect();
    }
    //private void OnDrawGizmos()
    //{
    //    if (needleTransform == null) return;

    //    Gizmos.color = Color.black;
    //    Gizmos.DrawWireSphere(needleTransform.position, detectionRadius);
    //}

    public void RegisterService()
    {
        ServiceLocator.RegisterService<INeedleDetector>(this);
        GameEvents.NeedleDetectorEvents.onSetRadiusValue.RegisterEvent(SetRadiusValue);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleDetector>(this);
        GameEvents.NeedleDetectorEvents.onSetRadiusValue.UnregisterEvent(SetRadiusValue);

    }
    public void UndoLastConnectedPoint()
    {
        if (pointsDetected.Count == 0) return;

        //detect = false;
        SewPoint s = null;
        s = pointsDetected[pointsDetected.Count - 1];
        s.GetComponent<Collider>().enabled = true;
        s.pointMesh.enabled = true;
        s.connected = false;
        s.Selected(false);
        s.ChangeTextColor(Color.white);
        pointsDetected.Remove(s);
    }
    public SewPoint GetDetectedPoint(int index)
    {
        return pointsDetected[index];
    }
}
