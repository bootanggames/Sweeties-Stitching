using UnityEngine;

public class RewardSystemManager : MonoBehaviour, IRewardSystem
{
    [field: SerializeField] public PlushieCompletionRewardHandler plushieCompletionRewardHandler { get; private set; }
    [field: SerializeField] public LevelUpRewardHandler levelUpRewardHandler { get; private set; }

    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IRewardSystem>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IRewardSystem>(this);
    }
}
