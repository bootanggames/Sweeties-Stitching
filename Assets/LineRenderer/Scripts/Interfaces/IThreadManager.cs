using System.Collections.Generic;
using UnityEngine;

public interface IThreadManager : IGameService
{
    bool canUndo {  get; }
    float zVal { get; }
    bool threadInput { get; }
    List<Transform> detectedPoints {  get;}
    void AddPositionToLineOnDrag(Vector2 pos);
    void AddFirstPositionOnMouseDown(Vector2 headPos);
    void MoveThread(LineRenderer thread, bool isPrevThread);
    //List<Color> threadColor {  get; }
    int threadIndex {  get; }
    LineRenderer instantiatedLine {  get; }
    LineRenderer prevLine {  get; }
    Vector3 prevMouseDragPosition {  get; }
    Transform lastConnectedPoint {  get; }
    void SetLastConnectedPosition(Transform t);
    void ScaleDownAllPoints();
    int pointIndex { get; set; }
    void UpdateSpoolThreadLastPoint(float lerpSpeed);
    void SetUndoValue(bool val);
}
