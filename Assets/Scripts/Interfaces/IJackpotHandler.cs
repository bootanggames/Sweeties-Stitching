
using System.Collections.Generic;
using UnityEngine;

public interface IJackpotHandler : IGameService
{
    GameObject jackpotPrefab {  get; }
    GameObject mainCamera {  get; }
    GameObject mainMenuCanvas {  get; }
    GameObject roomDecorCanvas {  get; }
    GameObject roomBg {  get; }
    GameObject plushieInventoryCanvas {  get; }
    void ShowJackPotScreen();
    void CloseJackpotScreen();
    void CloseRewardScreen();
    List<ItemsMetaData> earnedItems { get; }
}
