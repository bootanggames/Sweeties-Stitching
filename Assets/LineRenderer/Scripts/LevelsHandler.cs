using System.Collections.Generic;
using UnityEngine;

public class LevelsHandler : Singleton<LevelsHandler>, ILevelHandler
{
    [field:SerializeField] public List<LevelStructure> levelStructure {  get; private set; }
    //[field:SerializeField] public List<GameObject> levels {  get; private set; }
    [field: SerializeField] public int levelIndex { get; private set; }
    [field: SerializeField] public int plushieIndex { get; private set; }
    public LevelStructure currentLevelData { get; private set; }
    public Level_Metadata currentLevelMeta { get; private set; }
    int totalCoins;
    public override void SingletonAwake()
    {
        base.SingletonAwake();
        RegisterService();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        UnRegisterService();
    }
    public override void SingletonStart()
    {
        base.SingletonStart();
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler != null)
        {
            if(gameHandler.saveProgress)
                LoadLastSavedProgress();
            else
                PlayerPrefs.SetInt("StitchedCount", 0);
        }
        levelIndex = PlayerPrefs.GetInt("Level");
        currentLevelData = levelStructure[levelIndex];
        plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");

        currentLevelMeta = currentLevelData.plushie[plushieIndex];
        if (currentLevelMeta.noOfStitchedPart.Equals(currentLevelMeta.levelScriptable.totalParts))
        {
            currentLevelMeta.ResetLevel();
            currentLevelMeta.LevelInitialisation();
        }
        SetLevel();
        Invoke("GetCoins", 0.25f);
    }
    void GetCoins()
    {
        totalCoins = PlayerPrefs.GetInt("Coins");
        CancelInvoke("GetCoins");
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
    public void SetLevelPlushiePref(int val)
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        PlayerPrefs.SetInt("Level_"+levelIndex+"_Plushie", val);
    }
    public void SetLevelLockState(int levelIndex, int plushieIndex, int val)
    {
        PlayerPrefs.SetInt("Level_" + levelIndex + "Plushie_" + plushieIndex, val);
        PlayerPrefs.SetInt(currentLevelMeta.levelScriptable.levelName, val);
    }
    public void UpdatePlushieInventory(int l_index, int totalCompletedPlushie)
    {
        for (int i = 0; i <= totalCompletedPlushie; i++)
        {
            string completedLevelName = levelStructure[l_index].plushie[i].levelScriptable.levelName;
            if (PlayerPrefs.HasKey(completedLevelName))
            {
                int val = PlayerPrefs.GetInt(completedLevelName);
                if (val <= 0)
                    PlayerPrefs.SetInt(completedLevelName, 1);
            }
            else
                PlayerPrefs.SetInt(completedLevelName, 1);

        }
    }
    public void LevelIncrementProcess()
    {
        PlayerPrefs.SetInt("SaveProgress", 0);
        levelIndex = PlayerPrefs.GetInt("Level");
        plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        //if (plushieIndex < levelStructure[levelIndex].plushie.Length)
        plushieIndex++;

        if (plushieIndex >= levelStructure[levelIndex].plushie.Length)
        {
            levelIndex++;
            plushieIndex = 0;
            if (levelIndex >= levelStructure.Count)
            {
                levelIndex = 0;
                plushieIndex = 0;
                levelStructure[levelIndex].completed = false;
            }
            else
                levelStructure[levelIndex].completed = true;
        }

        SetPref(levelIndex);
        SetLevelPlushiePref(plushieIndex);
        SetLevelLockState(levelIndex, plushieIndex, 1);
        currentLevelData = levelStructure[levelIndex];
        currentLevelMeta = currentLevelData.plushie[plushieIndex];
        //currentLevelMeta.gameObject.SetActive(true);
        currentLevelMeta.ResetLevel();
        currentLevelMeta.LevelInitialisation();
        SetLevel();
        currentLevelMeta.gameObject.SetActive(false);

    }
    public void NextPlushie()
    {
        var coinHandler = ServiceLocator.GetService<ICoinsHandler>();
     
        var connectionHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if (connectionHandler != null) connectionHandler.DeleteAllThreadLinks();

        int rewardedCoins = currentLevelMeta.levelScriptable.levelReward;
        LevelIncrementProcess();
        //int TotalEarned = totalCoins + rewardedCoins;
        //PlayerPrefs.SetInt("Coins", TotalEarned);
        //if (coinHandler != null)
        //{
        //    coinHandler.ResetCoinList();
        //    coinHandler.UpdateCoins(TotalEarned);
        //}
        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
        {
            canvasHandler.startText.transform.localScale = Vector3.zero;
            canvasHandler.startText.SetActive(true);
            canvasHandler.stitchProgress.text = "0% Done";
            canvasHandler.stitchCountText.text = currentLevelMeta.noOfStitchesDone + " OF " + currentLevelMeta.levelScriptable.totalStitches;
        }
    
        var IThreadHandler = ServiceLocator.GetService<IThreadManager>();
        if(IThreadHandler != null)
            IThreadHandler.SetUndoValue(true);

    }
    public void SetLevel()
    {
        currentLevelData.DisableAllPlushies();
        currentLevelMeta.gameObject.SetActive(true);
    }
   
    void LoadLastSavedProgress()
    {
        levelIndex = PlayerPrefs.GetInt("Level");
        plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        currentLevelMeta = levelStructure[levelIndex].plushie[plushieIndex];
        if (currentLevelMeta != null)
        {
            int stitchedCountOfCurrentLevel =  PlayerPrefs.GetInt("StitchedCount");
            currentLevelMeta.noOfStitchesDone = stitchedCountOfCurrentLevel;
            currentLevelMeta.noOfStitchedPart = PlayerPrefs.GetInt("StitchedPartCount");
            currentLevelMeta.Delay();
            currentLevelMeta.CheckIfStitchedBeforeCompleteScreen();
        }
    }
}
