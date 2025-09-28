using UnityEngine;

public class NeedleMovement : MonoBehaviour,INeedleMovement
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform needle;

    private void OnEnable()
    {
        RegisterService();
        GameEvents.ThreadEvents.onInitialiseRope.RaiseEvent(needle.position);
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<INeedleMovement>(this);
        GameEvents.NeedleEvents.OnNeedleMovement.RegisterEvent(MoveNeedle);
        GameEvents.NeedleEvents.OnGettingCurrentPositionFromFixedStart.RegisterEvent(GetCurrentPositionFromStartPoint);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleMovement>(this);
        GameEvents.NeedleEvents.OnNeedleMovement.UnregisterEvent(MoveNeedle);
        GameEvents.NeedleEvents.OnGettingCurrentPositionFromFixedStart.UnregisterEvent(GetCurrentPositionFromStartPoint);
    }
    public Vector3 GetCurrentPositionFromStartPoint(Vector3 pos)
    {
        pos.z = 0;
        return (startPoint.position + pos);
    }
    public void MoveNeedle(Vector3 pos)
    {
        needle.position = pos;
    }
}
