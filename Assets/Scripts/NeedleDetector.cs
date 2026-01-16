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
    IPointConnectionHandler pointsHandler;
    IThreadManager threadHandler;
    private void OnEnable()
    {
        RegisterService();
        pointsHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        threadHandler = ServiceLocator.GetService<IThreadManager>();
    }
    private void Start()
    {
        pointsHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        threadHandler = ServiceLocator.GetService<IThreadManager>();
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
    public void ResetDetectedPointsList(List<SewPoint> list)
    {
        pointsDetected = list;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("StitchPoint"))
        {
            if (!detect) return;
            SewPoint sewPoint = other.GetComponent<SewPoint>();
            if (sewPoint.IsSelected()) return;
        
            if (sewPoint.metaData.connected) return;
            sewPoint.Selected(true);

            sewPoint.GetComponent<Collider>().enabled = false;
            sewPoint.name = sewPoint.transform.parent.name + "_sew_" + sewPoint.name;
            PlaySound();
            sewPoint.ChangeTextColor(Color.green);
            GameEvents.EffectHandlerEvents.onSelectionEffect.Raise(sewPoint.transform);
            GameEvents.ThreadEvents.onCreatingConnection.Raise(sewPoint);

            if (!pointsDetected.Contains(sewPoint))
                pointsDetected.Add(sewPoint);

            if (pointsHandler != null)
            {
                if (pointsDetected.Count > 0)
                {
                    if (pointsDetected.Count % 2 == 0)
                    {
                        if (threadHandler != null)
                            threadHandler.ScaleDownAllPoints();
                    }
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
   
    public void RegisterService()
    {
        ServiceLocator.RegisterService<INeedleDetector>(this);
        GameEvents.NeedleDetectorEvents.onSetRadiusValue.Register(SetRadiusValue);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleDetector>(this);
        GameEvents.NeedleDetectorEvents.onSetRadiusValue.UnRegister(SetRadiusValue);

    }
    public void UndoLastConnectedPoint()
    {
        if (pointsDetected.Count == 0) return;

        SewPoint s = null;
        s = pointsDetected[pointsDetected.Count - 1];
        s.GetComponent<Collider>().enabled = true;
        s.pointMesh.enabled = true;
        s.IsConnected(false, 0,Vector3.zero,"");
        s.Selected(false);
        s.ChangeTextColor(Color.white);
        pointsDetected.Remove(s);
    }
    public SewPoint GetDetectedPoint(int index)
    {
        return pointsDetected[index];
    }
}
