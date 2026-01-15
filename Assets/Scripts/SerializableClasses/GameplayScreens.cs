using System;
using UnityEngine;

[Serializable]
public struct GameplayScreens 
{
    [field: SerializeField] public GameObject gameCompleteBGCanvas { get; private set; }
    [field: SerializeField] public GameObject gameCompletePanel { get; private set; }
    [field: SerializeField] public GameObject sewnScreen { get; private set; }
    [field: SerializeField] public GameObject confettiEffectCanvas { get; private set; }
    [field: SerializeField] public GameObject goToHomeScreen { get; private set; }
    [field: SerializeField] public GameObject mainCanvas { get; private set; }
    [field: SerializeField] public GameObject plushiesInventoryMainObject { get; private set; }
    [field: SerializeField] public GameObject plushiesInventoryScreenObj { get; private set; }
    [field: SerializeField] public GameObject roomDecorScreen { get; private set; }
    [field: SerializeField] public GameObject storeScreen { get; private set; }
    [field: SerializeField] public GameObject storeBg { get; private set; }

}
