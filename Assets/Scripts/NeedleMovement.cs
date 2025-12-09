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
        GameEvents.NeedleEvents.OnNeedleMovement.Register(MoveNeedle);
        GameEvents.NeedleEvents.OnFetchingNeedlePosition.Register(GetPosition);
        GameEvents.NeedleEvents.onGettingNeedleTransform.Register(GetNeedle);
        GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.Register(HandleNeedleActiveStatus);
        GameEvents.NeedleEvents.onNeedleRotation.Register(NeedleRotation);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleMovement>(this);
        GameEvents.NeedleEvents.OnNeedleMovement.UnRegister(MoveNeedle);
        GameEvents.NeedleEvents.OnFetchingNeedlePosition.UnRegister(GetPosition);
        GameEvents.NeedleEvents.onGettingNeedleTransform.UnRegister(GetNeedle);
        GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.UnRegister(HandleNeedleActiveStatus);
        GameEvents.NeedleEvents.onNeedleRotation.UnRegister(NeedleRotation);

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

    public void ChangeMinThreshHoldValue(float val)
    {
        minRotationThreshold = val;
    }
}
