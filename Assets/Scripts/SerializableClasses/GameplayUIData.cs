using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct GameplayUIData 
{
    [field: SerializeField] public GameObject completeStitchedPlushie { get; private set; }
    [field: SerializeField] public TextMeshProUGUI stitchCountText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI stitchProgress { get; private set; }
    [field: SerializeField] public GameObject startText { get; private set; }
    [field: SerializeField] public GameObject undoHighLight { get; private set; }
    [field: SerializeField] public GameObject sewnTextImage { get; private set; }
    [field: SerializeField] public AudioSource audioSourceForBG { get; private set; }
    [field: SerializeField] public GameObject alertTextObj { get; private set; }
}
