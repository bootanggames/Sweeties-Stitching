using Unity.Cinemachine;
using UnityEngine;

public interface ICameraManager : IGameService
{
    CinemachineSequencerCamera cameraSequencer {  get; }
    CinemachineBlendDefinition blend {  get; }
    float hold {  get; }
    int cameraIndex {  get; }

    CinemachineVirtualCameraBase neckCamera { get; }
    CinemachineVirtualCameraBase leftEyeCamera { get; }
    CinemachineVirtualCameraBase leftEarCamera { get; }
    CinemachineVirtualCameraBase rightEarCamera { get; }
    CinemachineVirtualCameraBase rightEyeCamera { get; }
    CinemachineVirtualCameraBase rightArmCamera { get; }
    CinemachineVirtualCameraBase rightLegCamera { get; }
    CinemachineVirtualCameraBase leftLegCamera { get; }
    CinemachineVirtualCameraBase leftArmCamera { get; }
    CinemachineVirtualCameraBase gameCompleteCamera { get; }
    CinemachineVirtualCameraBase gameHalfProgressZoomOutCamera { get; }

}
