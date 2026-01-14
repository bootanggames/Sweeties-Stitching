using System;
using System.Transactions;
using UnityEngine;

public class LevelUpRewardHandler : MonoBehaviour
{
   [field:SerializeField]public LevelUpRewardList rewardList {  get; private set; }
    const float scale_LevelCoinsEV = 0.71f;
    const float scale_DailyCoinsEV = 0.32f;
    const int roundingUnit = 10;

    [Header("-------- Level Up Jackpot--------")]
    const float coinsShare = 0.5f;
    const float decorShare = 0.5f;
    [Header("-------- Level Up Reward Coins--------")]
    const float smallCoinsMultiplier = 0.5f;
    const float mediumCoinsMultiplier = 1.0f;
    const float hugeCoinsMultiplier = 2.0f;
    [Header("-------- Daily Reward Coins--------")]
    const float dailySmallCoinsMultiplier = 0.2f;
    const float dailyMediumCoinsMultiplier = 0.5f;
    const float dailyHugeCoinsMultiplier = 1.0f;
    [Header("--------Jackpot Probabilities--------")]
    const float smallTier = 0.7f;
    const float mediumTier = 0.25f;
    const float hugeTier = 0.05f;
    double RoundUpRewardValue(double ev)
    {
        int digits = -(int)Math.Floor(Math.Log10(roundingUnit));
        double factr = Math.Pow(roundingUnit, -digits);
        double result = Math.Ceiling(ev / factr) * factr;
        return result;
    }

    public double EVLevelJackpot_CoinsOnly()
    {
        //coinsshare * (smalltier * smallCoins + mediumtier * mediumCoins+ hugetier* HugeCoins)
        double evCoinsOnly = coinsShare * (smallTier * CalculateLevelUpSmallCoinsReward() + mediumTier * CalculateLevelUpMediumCoinsReward()
            + hugeTier * CalculateLevelUpHugeCoinsReward());
        return evCoinsOnly;
    }
    public double EVLevelJackpot_DecorOnly()
    {
        //decorshare * (smalltier  + mediumtier + hugetier) * DecorPriceatLevel
        int levelIndex = PlayerPrefs.GetInt("Level");
        double evDecorOnly = 0;
        if (levelIndex > 0)
        {
            int amount = int.Parse(rewardList.levelUpReward[levelIndex - 1].DecorCoinsPerCompletion);
            evDecorOnly = decorShare * (smallTier + mediumTier + hugeTier) * amount;
        }
        return evDecorOnly;
    }

    public double EVLevelJackpot_TotalValue()
    {
        //decorshare * (smalltier  + mediumtier + hugetier) * DecorPriceatLevel
        double totalVal = EVLevelJackpot_CoinsOnly() + EVLevelJackpot_DecorOnly();
        return totalVal;
    }

    public double CalculateLevelUpSmallCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        double rewardAmount = 0;
        if (levelIndex > 0)
        {
            int amount = int.Parse(rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            rewardAmount = amount * smallCoinsMultiplier * scale_LevelCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return rewardAmount;
    }

    public double CalculateLevelUpMediumCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            int amount = int.Parse(rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * mediumCoinsMultiplier * scale_LevelCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;

    }

    public double CalculateLevelUpHugeCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            int amount = int.Parse(rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * hugeCoinsMultiplier * scale_LevelCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;
    }

    public double CalculateDailySmallCoinsReward()
    {
        //=ROUND(LevelUpCoinsReward*SmallCoinsMultiplierForDailyReward*Scale_DailyCoinsEV, -INT(Log10(RoundingUnit)))
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            int amount = int.Parse(rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * dailySmallCoinsMultiplier * scale_DailyCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;
    }
    public double CalculateDailyMediumCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            int amount = int.Parse(rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * dailyMediumCoinsMultiplier * scale_DailyCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;
    }

    public double CalculateDailyHugeCoinsReward()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        if (levelIndex > 0)
        {
            int amount = int.Parse(rewardList.levelUpReward[levelIndex - 1].CoinsPerCompletion);
            double rewardAmount = amount * dailyHugeCoinsMultiplier * scale_DailyCoinsEV;
            return RoundUpRewardValue(rewardAmount);
        }
        return 0;
    }
}
