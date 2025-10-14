using UnityEngine;

public interface IGameHandler :IGameService
{
   GameStates gameStates {  get; }

}
