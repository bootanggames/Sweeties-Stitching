using UnityEngine;

public interface IThreadManager : IGameService
{
    void AddPositionToLineOnDrag(Vector2 pos);
    void AddFirstPositionOnMouseDown(Vector2 headPos);
    void MoveThread(LineRenderer thread, bool isPrevThread);
}
