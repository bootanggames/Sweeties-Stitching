using UnityEngine;

public interface IThreadManager : IGameService
{
    public void InitializeRope(Vector3 needlePos);
    public void AddPositionToLine(Vector3 pos);
}
