using DG.Tweening;
using UnityEngine;

public class NeedleMovement : MonoBehaviour,INeedleMovement
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform needleParent;
    [SerializeField] float needleRotationSpeed;
    [SerializeField] float angleOffset;
    [SerializeField] float minRotationThreshold;
    [SerializeField] Ease ease;
    [SerializeField] Transform needle;
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<INeedleMovement>(this);
        GameEvents.NeedleEvents.OnNeedleMovement.RegisterEvent(MoveNeedle);
        GameEvents.NeedleEvents.OnFetchingNeedlePosition.RegisterEvent(GetPosition);
        GameEvents.NeedleEvents.onGettingNeedleTransform.RegisterEvent(GetNeedle);
        GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.RegisterEvent(HandleNeedleActiveStatus);
        GameEvents.NeedleEvents.onNeedleRotation.RegisterEvent(NeedleRotation);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleMovement>(this);
        GameEvents.NeedleEvents.OnNeedleMovement.UnregisterEvent(MoveNeedle);
        GameEvents.NeedleEvents.OnFetchingNeedlePosition.UnregisterEvent(GetPosition);
        GameEvents.NeedleEvents.onGettingNeedleTransform.UnregisterEvent(GetNeedle);
        GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.UnregisterEvent(HandleNeedleActiveStatus);
        GameEvents.NeedleEvents.onNeedleRotation.UnregisterEvent(NeedleRotation);

    }

    public void MoveNeedle(Vector3 pos)
    {
        needleParent.position = pos;
    }

    public Vector3 GetPosition()
    {
        return needleParent.position;
    }
    
    Transform GetNeedle()
    {
        return needleParent;
    }

    public void HandleNeedleActiveStatus(bool active)
    {
        needleParent.gameObject.SetActive(active);
    }
    void NeedleRotation(float magnitude, Vector3 _direction)
    {
        if (magnitude > minRotationThreshold)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            needleParent.transform.DOKill();
            needleParent.transform.DORotate(new Vector3(0, 0, angle - angleOffset), needleRotationSpeed).SetEase(ease);
        }
    }

    public void NeedleSize(float val)
    {
        needle.localScale = new Vector3(val, val, val);
    }
}
