using System.Collections.Generic;
using UnityEngine;

public interface ILevelHandler : IGameService
{
    List<GameObject> levels {  get; }
    int levelIndex { get; }
    Level_Metadata currentLevelMeta { get; }
}
