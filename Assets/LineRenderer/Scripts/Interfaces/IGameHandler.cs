using UnityEngine;

public interface IGameHandler :IGameService
{
   GameStates gameStates {  get; }
<<<<<<< Updated upstream
    bool saveProgress { get; }
=======
>>>>>>> Stashed changes
    void SwitchGameState(GameStates state);
}
