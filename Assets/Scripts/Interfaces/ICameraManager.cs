using Unity.Cinemachine;
using UnityEngine;

public interface ICameraManager : IGameService
{
    CinemachineSequencerCamera cameraSequencer {  get; }
    CinemachineBlendDefinition blend {  get; }
    float hold {  get; }
    int cameraIndex {  get; }


    CameraReferences cameraData { get; }
    void RepositionCamera(Transform camera, Vector3 pos);
    
}
