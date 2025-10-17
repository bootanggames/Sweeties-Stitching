using System.Collections.Generic;
using UnityEngine;

public interface IThreadManager : IGameService
{
    bool threadInput { get; }
    bool freeForm { get; }
    List<Transform> detectedPoints {  get;}
    void AddPositionToLineOnDrag(Vector2 pos);
    void AddFirstPositionOnMouseDown(Vector2 headPos);
    void MoveThread(LineRenderer thread, bool isPrevThread);
    List<Color> threadColor {  get; }
    int threadIndex {  get; }

}
