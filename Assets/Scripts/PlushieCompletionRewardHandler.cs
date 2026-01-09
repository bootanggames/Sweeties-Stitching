using System.Collections.Generic;
using UnityEngine;

public class PlushieCompletionRewardHandler : MonoBehaviour
{
    public TextAsset rewardFile;
    [SerializeField] List<PlushieCompletionRewardParameters> plushieCompletionReward;

    private void Start()
    {
        SaveDataUsingJson.instance.LoadData<PlushieCompletionRewardParameters>("PlushieCompletionReward", "RewardSystem");
    }
}
