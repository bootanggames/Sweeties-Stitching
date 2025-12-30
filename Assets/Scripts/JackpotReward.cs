using UnityEngine;

[CreateAssetMenu(fileName = "JackpotReward", menuName = "Jackpot/Reward")]
public class JackpotReward : ScriptableObject
{
    [field: SerializeField] public RewardType rewardType { get; private set; }
    [field: SerializeField] public Sprite rewardIcon { get; private set; }
    [field: SerializeField] public float rewardAmount { get; private set; }
    [field: SerializeField] public float rewardProbability { get; private set; }
}
