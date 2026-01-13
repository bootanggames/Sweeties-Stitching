using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JackpotReward", menuName = "Jackpot/Reward")]
public class JackpotReward : ScriptableObject
{
    [field: SerializeField] public int rewardID { get; private set; }
    [field: SerializeField] public RewardType rewardType { get; private set; }
    [field: SerializeField] public Sprite rewardIcon { get; private set; }
    [field: SerializeField] private float rewardAmount { get;}
    [field: SerializeField] public List<SlotRewardProbability> slotRewardProbability { get; private set; }
    [field: SerializeField] public List<ItemsMetaData> items {  get; private set; }
    IRewardSystem rewardSystem;

    public ItemsMetaData GetItem()
    {
        ItemsMetaData _randomItem = null;
        int r = Random.Range(0, items.Count);
        _randomItem = items[r];
        return _randomItem;
    }

    public float GetRewardAmount()
    {
        rewardSystem = ServiceLocator.GetService<IRewardSystem>();
        return rewardAmount;
    }
}
