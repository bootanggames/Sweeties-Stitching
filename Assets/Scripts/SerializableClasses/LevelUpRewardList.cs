using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelUpRewardList 
{
    [field: SerializeField]public List<LevelUpRewardParameters> levelUpReward { get; private set; }
}
