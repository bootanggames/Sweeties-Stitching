using MoreMountains.NiceVibrations;
using UnityEngine;

public class NeedleDetector : MonoBehaviour, INeedleDetector
{
    [SerializeField]Transform needleTransform;
    [SerializeField] LayerMask detectLayer;
    [field: SerializeField] public float detectionRadius {  get; private set; }
    [field: SerializeField] public float minDetectionRadius { get; private set; }
    [field: SerializeField] public float maxDetectionRadius { get; private set; }

    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    void Update()
    {
        DetectPoints();
    }
    void SetRadiusValue(float val)
    {
        detectionRadius = val;
    }
    void DetectPoints()
    {
        Collider[] colliders = Physics.OverlapSphere(needleTransform.position, detectionRadius, detectLayer);
        if (colliders.Length <= 0) return;
        SewPoint sewPoint = colliders[0].GetComponent<SewPoint>();
        if (sewPoint.IsSelected()) return;
        sewPoint.Selected(true);
        sewPoint.GetComponent<MeshRenderer>().enabled = false;
        sewPoint.GetComponent<Collider>().enabled = false;
        sewPoint.name = "sew"+ sewPoint.name;
        PlaySound();
        GameEvents.ThreadEvents.onCreatingConnection.RaiseEvent(sewPoint);
    }
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.coinCollection;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
    private void OnDrawGizmos()
    {
        if (needleTransform == null) return;

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(needleTransform.position, detectionRadius);
    }

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
}
