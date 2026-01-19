using System;
using UnityEngine;

[Serializable]
public struct JackpotData
{
    [field: SerializeField] public GameObject jackpotPrefab { get; private set; }
    [field: SerializeField] public GameObject mainCamera { get; private set; }
    [field: SerializeField] public GameObject roomDecorCanvas { get; private set; }
    [field: SerializeField] public GameObject plushieInventoryCanvas { get; private set; }

}
