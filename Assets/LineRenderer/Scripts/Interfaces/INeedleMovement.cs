using UnityEngine;

public interface INeedleMovement : IGameService
{
    public void MoveNeedle(Vector3 pos);
}
