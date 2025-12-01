using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : Singleton<GameHandler>, IGameHandler
{
    [field: SerializeField] public GameStates gameStates {  get; private set; }
    [field: SerializeField] public bool saveProgress {  get; private set; }

    public override void SingletonAwake()
    {
        Time.timeScale = 1;
        base.SingletonAwake();
        RegisterService();
        int saveState = PlayerPrefs.GetInt("SaveProgress");
        if(saveState == 1)
            saveProgress = true;
        else
            saveProgress = false;
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        UnRegisterService();
    }

    public void Home(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SwitchGameState(GameStates state)
    {
        switch (state)
        {
            case GameStates.None:
                break;
            case GameStates.Gamestart:
                gameStates = GameStates.Gamestart;
                break;
            case GameStates.Gamepause:
                gameStates = GameStates.Gamepause;
                break;
            case GameStates.Gamecomplete:
                gameStates = GameStates.Gamecomplete;
                break;
            case GameStates.Gamefail:
                gameStates = GameStates.Gamefail;
                break;
            case GameStates.ThreadSpoolBuyScreen:
                gameStates = GameStates.ThreadSpoolBuyScreen;
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

    public void RegisterService()
    {
        ServiceLocator.RegisterService<IGameHandler>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IGameHandler>(this);
    }

    public void DontSaveProgress()
    {
        PlayerPrefs.SetInt("SaveProgress", 0);
        Home("HomeScreen");
    }

    public void SaveGameProgress()
    {
        PlayerPrefs.SetInt("SaveProgress", 1);
        PlayerPrefs.SetInt("StitchedPartCount", LevelsHandler.instance.currentLevelMeta.noOfStitchedPart);
        LevelsHandler.instance.currentLevelMeta.UpdateLinks();
        PlayerPrefs.SetInt("StitchedCount", LevelsHandler.instance.currentLevelMeta.noOfLinks);
        Home("HomeScreen");
    }
}
