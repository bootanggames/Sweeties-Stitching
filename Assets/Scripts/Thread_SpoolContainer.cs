using UnityEngine;

public class Thread_SpoolContainer : MonoBehaviour
{
    public void SetGameState(string screen)
    {
        if (GameHandler.instance == null) return;
        if (screen.Equals("ThreadStore"))
            GameHandler.instance.SwitchGameState(GameStates.ThreadSpoolBuyScreen);
        else if(screen.Equals("ResumeToGame"))
            GameHandler.instance.SwitchGameState(GameStates.Gamestart);

    }
}
