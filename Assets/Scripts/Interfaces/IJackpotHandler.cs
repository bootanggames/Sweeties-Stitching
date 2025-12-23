
using UnityEngine;

public interface IJackpotHandler : IGameService
{
    GameObject jackpotPrefab {  get; }
    GameObject gameplayCamera {  get; }
    GameObject gameplayCanvas {  get; }
    GameObject gameCompleteCanvas {  get; }
    GameObject plushieInventoryCanvas {  get; }
    void ShowJackPotScreen();
    void CloseJackpotScreen();
    void CloseRewardScreen();
}
