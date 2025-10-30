using System.Collections.Generic;
using UnityEngine;

public class LevelsHandler : Singleton<LevelsHandler>, ILevelHandler
{
    [field:SerializeField] public List<GameObject> levels {  get; private set; }
    [field: SerializeField] public int levelIndex { get; private set; }

    public Level_Metadata currentLevelMeta { get; private set; }

    public override void SingletonAwake()
    {
        base.SingletonAwake();
        RegisterService();
        levelIndex = PlayerPrefs.GetInt("Level");
        currentLevelMeta = levels[levelIndex].GetComponent<Level_Metadata>();
        SetLevel();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        UnRegisterService();
    }
    public override void SingletonStart()
    {
        base.SingletonStart();
<<<<<<< Updated upstream
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler != null)
        {
            if(gameHandler.saveProgress)
                LoadLastSavedProgress();
        }
=======
        LoadLastSavedProgress();
>>>>>>> Stashed changes
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<ILevelHandler>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ILevelHandler>(this);
    }
    public void SetPref(int val)
    {
        PlayerPrefs.SetInt("Level", val);
    }
    public void NextPlushie()
    {
        var coinHandler = ServiceLocator.GetService<ICoinsHandler>();
        if(coinHandler != null)
            coinHandler.ResetCoinList();
        var connectionHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if (connectionHandler != null) connectionHandler.DeleteAllThreadLinks();
     
        levelIndex = PlayerPrefs.GetInt("Level");
        levelIndex++;
        if (levelIndex >= levels.Count)
            levelIndex = 0;
        SetPref(levelIndex);
       currentLevelMeta = levels[levelIndex].GetComponent<Level_Metadata>();

        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
        {
            //canvasHandler.tapToStartButton.SetActive(true);
            canvasHandler.startText.SetActive(true);
            canvasHandler.startText.transform.localScale = Vector3.zero;
            canvasHandler.startText.GetComponent<Animate>().StartTextAnimation();
            canvasHandler.stitchCountText.text = currentLevelMeta.noOfCorrectLinks + " OF " + currentLevelMeta.totalCorrectLinks;
        }
        
        SetLevel();
    }
    public void SetLevel()
    {
        foreach(GameObject g in levels)
        {
            g.SetActive(false);
        }
        currentLevelMeta.gameObject.SetActive(true);
    }
   
    void LoadLastSavedProgress()
    {
        if(currentLevelMeta != null)
        {
            int stitchedCountOfCurrentLevel =  PlayerPrefs.GetInt("StitchedCount");
            currentLevelMeta.noOfCorrectLinks = stitchedCountOfCurrentLevel;
<<<<<<< Updated upstream
            currentLevelMeta.noOfStitchedPart = PlayerPrefs.GetInt("StitchedPartCount");
=======
            currentLevelMeta.noOfStitchedPart = PlayerPrefs.GetInt("StitchedCount");
>>>>>>> Stashed changes
        }
    }
}
