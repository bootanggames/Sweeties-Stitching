using System.Collections.Generic;
using UnityEngine;

public interface INeedleDetector : IGameService
{
    float detectionRadius { get; }
    float minDetectionRadius { get; }
    float maxDetectionRadius { get; }
    bool detect { get; set; }
    List<SewPoint> pointsDetected {  get; }
    void UndoLastConnectedPoint();
    SewPoint GetDetectedPoint(int index);
    void ResetDetectedPointsList(List<SewPoint> list);
}
