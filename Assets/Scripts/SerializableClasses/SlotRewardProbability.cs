using System;
using UnityEngine;

[Serializable]
public class SlotRewardProbability
{
    [field:SerializeField] public int slotId {  get; private set; }
    [field:SerializeField] public int rewardProbability {  get; private set; }
}
