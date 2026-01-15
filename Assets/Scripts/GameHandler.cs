using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : Singleton<GameHandler>
{
    [field: SerializeField] public GameStates currentActiveScreen {  get; private set; }
    [field: SerializeField] public GameStates previousScreen {  get; private set; }
    [field: SerializeField] public bool saveProgress {  get; private set; }
    public override void SingletonAwake()
    {
        Time.timeScale = 1;
        base.SingletonAwake();
        int saveState = PlayerPrefs.GetInt("SaveProgress");
        if(saveState == 1)
            saveProgress = true;
        else
            saveProgress = false;
    }
    
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }

    public void Home(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SwitchGameState(GameStates state)
    {
        previousScreen = currentActiveScreen;
        switch (state)
        {
            case GameStates.LevelObjectiveScreen:
                currentActiveScreen = GameStates.LevelObjectiveScreen;
                break;
            case GameStates.Gamestart:
                currentActiveScreen = GameStates.Gamestart;
                break;
            case GameStates.Gamepause:
                currentActiveScreen = GameStates.Gamepause;
                break;
            case GameStates.Gamecomplete:
                currentActiveScreen = GameStates.Gamecomplete;
                break;
            case GameStates.Gamefail:
                currentActiveScreen = GameStates.Gamefail;
                break;
            case GameStates.ThreadSpoolBuyScreen:
                currentActiveScreen = GameStates.ThreadSpoolBuyScreen;
                break;
            case GameStates.JackpotScreen:
                currentActiveScreen = GameStates.JackpotScreen;
                break;
            case GameStates.PlushieInventorySceen:
                currentActiveScreen = GameStates.PlushieInventorySceen;
                break;
            case GameStates.RoomDecorScreen:
                currentActiveScreen = GameStates.RoomDecorScreen;
                break;
            case GameStates.StoreScreen:
                currentActiveScreen = GameStates.StoreScreen;
                break;
        }
    }
  
    public void GameComplete()
    {
        GameCompleteSoundEffect();
    }

    void GameCompleteSoundEffect()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }

    public void DontSaveProgress()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("SaveProgress", 0);
        Home("HomeScreen");
    }

    public void SaveGameProgress()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("SaveProgress", 1);
        PlayerPrefs.SetInt("StitchedPartCount", LevelsHandler.instance.currentLevelMeta.noOfStitchedPart);
        LevelsHandler.instance.currentLevelMeta.UpdateLinks();
        PlayerPrefs.SetInt("StitchedCount", LevelsHandler.instance.currentLevelMeta.noOfStitchesDone);
        Home("HomeScreen");
    }
   
}
