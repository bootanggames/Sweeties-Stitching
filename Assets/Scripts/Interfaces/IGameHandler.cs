using UnityEngine;

public interface IGameHandler :IGameService
{
   GameStates gameStates {  get; }
    bool saveProgress { get; }
    void SwitchGameState(GameStates state);
}
