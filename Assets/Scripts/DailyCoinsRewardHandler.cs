using System;
using UnityEngine;

public class DailyCoinsRewardHandler : MonoBehaviour
{
    const float scale_DailyCoinsEV = 0.32f;
    const int roundingUnit = 10;

    [Header("--------Daily Jackpot Probabilities--------")]
    const float smallTier = 0.2f;
    const float mediumTier = 0.5f;
    const float hugeTier = 1.0f;

    double RoundUpRewardValue(double ev)
    {
        int digits = -(int)Math.Floor(Math.Log10(roundingUnit));
        double factr = Math.Pow(roundingUnit, -digits);
        double result = Math.Ceiling(ev / factr) * factr;
        return result;
    }

    public double CalculateLevelUpSmallCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            LevelUpRewardHandler levelupReward = this.GetComponent<LevelUpRewardHandler>();
            int amount = int.Parse(levelupReward.rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * smallTier * scale_DailyCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;
    }

    public double CalculateLevelUpMediumCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            LevelUpRewardHandler levelupReward = this.GetComponent<LevelUpRewardHandler>();

            int amount = int.Parse(levelupReward.rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * mediumTier * scale_DailyCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;

    }

    public double CalculateLevelUpHugeCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            LevelUpRewardHandler levelupReward = this.GetComponent<LevelUpRewardHandler>();

            int amount = int.Parse(levelupReward.rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * hugeTier * scale_DailyCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;
    }
}
