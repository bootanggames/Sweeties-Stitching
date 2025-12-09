using System;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class CurrencyEntity
{
    [field: SerializeField] public CurrencyName ResourceName { get; set; }
    [field: SerializeField] public int Value { get; set; }
}