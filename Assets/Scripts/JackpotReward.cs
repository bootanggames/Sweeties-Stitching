using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JackpotReward", menuName = "Jackpot/Reward")]
public class JackpotReward : ScriptableObject
{
    [field: SerializeField] public int rewardID { get; private set; }
    [field: SerializeField] public RewardType rewardType { get; private set; }
    [field: SerializeField] public Sprite rewardIcon { get; private set; }
    [field: SerializeField] public float rewardAmount { get; private set; }
    [field: SerializeField] public float rewardProbability { get; private set; }
    [field: SerializeField] public List<ItemsMetaData> items {  get; private set; }
    public ItemsMetaData GetItem()
    {
        ItemsMetaData _randomItem = null;
        int r = Random.Range(0, items.Count);
        _randomItem = items[r];
        return _randomItem;
    }
}
