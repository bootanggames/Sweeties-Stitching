using DG.Tweening;
using UnityEngine;

public class NeedleMovement : MonoBehaviour,INeedleMovement
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform needle;
    [SerializeField] float needleRotationSpeed;
    [SerializeField] float angleOffset;
    [SerializeField] float minRotationThreshold;
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
        needle.position = pos;
    }

    public Vector3 GetPosition()
    {
        return needle.position;
    }
    
    Transform GetNeedle()
    {
        return needle;
    }

    public void HandleNeedleActiveStatus(bool active)
    {
        needle.gameObject.SetActive(active);
    }
    void NeedleRotation(float magnitude, Vector3 _direction)
    {
        //Debug.LogError(" " + magnitude);
        if (magnitude > minRotationThreshold)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            needle.transform.DOKill();
            needle.transform.DORotate(new Vector3(0, 0, angle - angleOffset), needleRotationSpeed).SetEase(Ease.Linear);

            //Quaternion targetRotation = Quaternion.Euler(0, 0, angle - angleOffset);
            //needle.transform.rotation = Quaternion.RotateTowards(
            //    needle.transform.rotation,
            //    targetRotation,
            //    needleRotationSpeed * Time.deltaTime
            //);
        }

    }
}
