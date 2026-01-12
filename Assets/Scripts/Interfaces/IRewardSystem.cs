using UnityEngine;

public interface IRewardSystem : IGameService
{
    PlushieCompletionRewardHandler plushieCompletionRewardHandler {  get;}
    LevelUpRewardHandler levelUpRewardHandler {  get;}
}
