using UnityEngine;
using UnityEngine.Splines;

public class ThreadManager : MonoBehaviour,IThreadManager
{
    [SerializeField] GameObject lineObj;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Vector3 lastPosition;

    private void OnEnable()
    {
        RegisterService();
    }

    private void OnDisable()
    {
        UnRegisterService();
    }
    public void AddPositionToLine(Vector3 pos)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition((lineRenderer.positionCount - 1), pos);
        lastPosition = pos;
        Debug.LogError(" " + pos);
    }

    public void InitializeRope(Vector3 needlePos)
    {
        lineRenderer.positionCount = 0;
        AddPositionToLine(needlePos);
    }

    public void RegisterService()
    {
        ServiceLocator.RegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.RegisterEvent(InitializeRope);
        GameEvents.ThreadEvents.onAddingPositionToRope.RegisterEvent(AddPositionToLine);

    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IThreadManager>(this);
        GameEvents.ThreadEvents.onInitialiseRope.UnregisterEvent(InitializeRope);
        GameEvents.ThreadEvents.onAddingPositionToRope.UnregisterEvent(AddPositionToLine);

    }


}
