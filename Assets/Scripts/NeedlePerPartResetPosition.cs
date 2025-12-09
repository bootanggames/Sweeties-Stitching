using UnityEngine;

public class NeedlePerPartResetPosition : MonoBehaviour, INeedleResetPositions
{
    [field: SerializeField] public Transform headNeedleResetPos {  get; private set; }
    [field: SerializeField] public Transform eyeRightNeedleResetPos { get; private set; }
    [field: SerializeField] public Transform earRightNeedleResetPos { get; private set; }
    [field: SerializeField] public Transform earLeftNeedleResetPos { get; private set; }
    [field: SerializeField] public Transform eyeLeftNeedleResetPos { get; private set; }
    [field: SerializeField] public Transform armLeftNeedleResetPos { get; private set; }
    [field: SerializeField] public Transform legLeftNeedleResetPos { get; private set; }
    [field: SerializeField] public Transform legRightNeedleResetPos { get; private set; }
    [field: SerializeField] public Transform armRightNeedleResetPos { get; private set; }

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
        ServiceLocator.RegisterService<INeedleResetPositions>(this);
    }
    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<INeedleResetPositions>(this);
    }
    
}
