using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct CoinsEffectData 
{
    [field: SerializeField] public GameObject coinBarForGameplayScreen { get; private set; }
    [field: SerializeField] public GameObject coinBar { get; private set; }
    [field: SerializeField] public TextMeshProUGUI coinsTextBox { get; private set; }
    [field: SerializeField] public GameObject coinSpritePrefab { get; private set; }
    [field: SerializeField] public TextMeshProUGUI coinsTextBoxGameplayScreen { get; private set; }
    [field: SerializeField] public Transform coinsGameplayTarget { get; private set; }
    [field: SerializeField] public GameObject coinPrefab { get; private set; }
    [field: SerializeField] public Transform coinsUiParent { get; private set; }
    [field: SerializeField] public Transform targetPointToMove { get; private set; }
    [field: SerializeField] public TextMeshProUGUI coinsEarned { get; private set; }
    [field: SerializeField] public AudioSource audioSource { get; private set; }
}
