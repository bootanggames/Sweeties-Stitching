using UnityEngine;

public interface INeedleMovement : IGameService
{
    void MoveNeedle(Vector3 pos);
    Vector3 GetPosition();
    void HandleNeedleActiveStatus(bool active);
    void NeedleSize(float val);
}
