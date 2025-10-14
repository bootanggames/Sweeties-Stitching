using UnityEngine;

public class NeedleMovement : MonoBehaviour,INeedleMovement
{
    [SerializeField] Transform startPoint;
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
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleMovement>(this);
        GameEvents.NeedleEvents.OnNeedleMovement.UnregisterEvent(MoveNeedle);
        GameEvents.NeedleEvents.OnFetchingNeedlePosition.UnregisterEvent(GetPosition);
        GameEvents.NeedleEvents.onGettingNeedleTransform.UnregisterEvent(GetNeedle);
        GameEvents.NeedleEvents.onNeedleActiveStatusUpdate.UnregisterEvent(HandleNeedleActiveStatus);

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
}
