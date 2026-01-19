using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct LevelObjectiveData 
{
    [field: SerializeField] public TextMeshProUGUI totalBodyPartsToStitch {  get; private set; }
    [field: SerializeField] public TextMeshProUGUI totalStitches { get; private set; }
    [field: SerializeField] public TextMeshProUGUI coinText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI coinEarnedText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI threadSpoolCount { get; private set; }
}
