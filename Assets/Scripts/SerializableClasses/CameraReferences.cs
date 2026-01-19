using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public struct CameraReferences 
{
    [field: SerializeField] public CinemachineVirtualCameraBase neckCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftEyeCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftEarCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightEarCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightEyeCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightArmCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase rightLegCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftLegCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase leftArmCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase gameCompleteCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCameraBase gameHalfProgressCamera { get; private set; }
}
