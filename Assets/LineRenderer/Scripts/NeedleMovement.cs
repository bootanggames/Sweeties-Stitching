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
        GameEvents.NeedleEvents.OnFetchingLastNeedlePosition.RegisterEvent(GetLastPosition);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleMovement>(this);
        GameEvents.NeedleEvents.OnNeedleMovement.UnregisterEvent(MoveNeedle);
        GameEvents.NeedleEvents.OnFetchingLastNeedlePosition.UnregisterEvent(GetLastPosition);
    }

    public void MoveNeedle(Vector3 pos)
    {
        needle.position = pos;
        GameEvents.ThreadEvents.onAddingPositionToRope.RaiseEvent(pos);
    }

  

    public Vector3 GetLastPosition()
    {
        return needle.position;
    }
}
