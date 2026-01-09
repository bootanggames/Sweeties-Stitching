using Mkey;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RewardList",menuName ="Jackpot/RewardList")]
public class JackpotAllRewards : ScriptableObject
{
    [SerializeField]private List<JackpotReward> rewardList;
    [SerializeField] List<PayLine> payoutTable;
    public JackpotReward GetRewardItem(RewardType type) =>
       rewardList.Find(x => x.rewardType == type);

    public List<JackpotReward> GetAllItems() => rewardList;
    public List<PayLine> GetPayoutTableValues() => payoutTable;
}
