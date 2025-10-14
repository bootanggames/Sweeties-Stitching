using Unity.Cinemachine;
using UnityEngine;

public static partial class GameEvents
{
    public static class CameraManagerEvents
    {
        public readonly static GameEvent<CinemachineVirtualCameraBase> onAddingCamera = new();
    }
}

