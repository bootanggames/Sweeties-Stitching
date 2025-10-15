using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour, ICameraManager
{
    [field: SerializeField] public CinemachineSequencerCamera cameraSequencer {  get; private set; }

    [field: SerializeField] public CinemachineBlendDefinition blend { get; private set; }

    [field: SerializeField] public float hold { get; private set; }

    public int cameraIndex { get; private set; }

    [field: SerializeField] public CinemachineVirtualCameraBase neckCamera     { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftEyeCamera  { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftEarCamera  { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightEarCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightEyeCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightArmCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightLegCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftLegCamera  { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftArmCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase gameCompleteCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase gameHalfProgressCamera { get; private set; }

    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void AddCamera(CinemachineVirtualCameraBase camera)
    {
        cameraSequencer.Instructions.Add(new CinemachineSequencerCamera.Instruction
        {
            Camera = camera,
            Blend = blend,
            Hold = hold
        });
       cameraIndex++;
    }

    public void RegisterService()
    {
        ServiceLocator.RegisterService<ICameraManager>(this);
        GameEvents.CameraManagerEvents.onAddingCamera.RegisterEvent(AddCamera);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ICameraManager>(this);
        GameEvents.CameraManagerEvents.onAddingCamera.UnregisterEvent(AddCamera);
    }
}
