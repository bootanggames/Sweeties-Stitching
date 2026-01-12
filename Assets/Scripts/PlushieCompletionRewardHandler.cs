using System.Collections.Generic;
using UnityEngine;

public class PlushieCompletionRewardHandler : MonoBehaviour
{
    //public TextAsset rewardFile;
    [field: SerializeField] public PlushieCompletionRewardsList plushieRewardList { get; private set; }

    private void Start()
    {
        //plushieRewardList = SaveDataUsingJson.instance.LoadData<PlushieCompletionRewardsList>("PlushieRewardTable.json", "RewardSystem");
    }
}
