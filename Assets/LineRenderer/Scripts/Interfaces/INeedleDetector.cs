using UnityEngine;

public interface INeedleDetector : IGameService
{
     float detectionRadius { get; }
     float minDetectionRadius { get; }
     float maxDetectionRadius { get; }

}
