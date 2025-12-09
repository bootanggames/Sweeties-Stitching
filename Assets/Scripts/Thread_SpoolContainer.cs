using UnityEngine;

public class Thread_SpoolContainer : MonoBehaviour
{
    public void SetGameState(string screen)
    {
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler == null) return;
        if (screen.Equals("ThreadStore"))
            gameHandler.SwitchGameState(GameStates.ThreadSpoolBuyScreen);
        else if(screen.Equals("ResumeToGame"))
            gameHandler.SwitchGameState(GameStates.Gamestart);

    }
}
