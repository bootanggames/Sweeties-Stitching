
using System.Collections.Generic;
using UnityEngine;

public interface IJackpotHandler : IGameService
{
    JackpotData jackpotData{ get; }
    void ShowJackPotScreen();
    void CloseJackpotScreen();
    void CloseRewardScreen();
    List<ItemsMetaData> earnedItems { get; }
}
