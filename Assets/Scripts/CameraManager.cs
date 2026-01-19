using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour, ICameraManager
{
    [field: SerializeField] public CinemachineSequencerCamera cameraSequencer {  get; private set; }

    [field: SerializeField] public CinemachineBlendDefinition blend { get; private set; }

    [field: SerializeField] public float hold { get; private set; }

    public int cameraIndex { get; private set; }

    [field: SerializeField] public CameraReferences cameraData  { get; private set; }
 

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
        GameEvents.CameraManagerEvents.onAddingCamera.Register(AddCamera);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ICameraManager>(this);
        GameEvents.CameraManagerEvents.onAddingCamera.UnRegister(AddCamera);
    }

    public void RepositionCamera(Transform camera, Vector3 pos)
    {
        camera.localPosition = pos;
    }
}
