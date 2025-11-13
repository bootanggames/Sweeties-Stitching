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
        plushieIndex = PlayerPrefs.GetInt("Plushie");
        currentLevelMeta = currentLevelData.plushie[plushieIndex];
        SetLevel();
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
        PlayerPrefs.SetInt("Plushie", val);
    }
    public void GetCurrentLevel()
    {
        int level = PlayerPrefs.GetInt("Level");
        int plushieIndex = PlayerPrefs.GetInt("Plushie");
    }

    public void UpdatePlushieInventory(int l_index, int totalCompletedPlushie)
    {
        for (int i = 0; i <= totalCompletedPlushie; i++)
        {
            string completedLevelName = levelStructure[l_index].plushie[i].levelName;
            if (PlayerPrefs.HasKey(completedLevelName))
            {
                int val = PlayerPrefs.GetInt(completedLevelName);
                if (val <= 0)
                    PlayerPrefs.SetInt(completedLevelName, 1);
            }
            else
                PlayerPrefs.SetInt(completedLevelName, 1);

            //Debug.LogError("updated " + completedLevelName);
        }
    }
    public void LevelIncrementProcess()
    {
        PlayerPrefs.SetInt("SaveProgress", 0);

        plushieIndex = PlayerPrefs.GetInt("Plushie");
        levelIndex = PlayerPrefs.GetInt("Level");
        if (plushieIndex < levelStructure[levelIndex].plushie.Length)
            plushieIndex++;

        // for plushie inventory update
        //UpdatePlushieInventory(levelIndex,(plushieIndex - 1));
        //

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

        currentLevelData = levelStructure[levelIndex];
        currentLevelMeta = currentLevelData.plushie[plushieIndex];
        currentLevelMeta.gameObject.SetActive(true);
        currentLevelMeta.ResetLevel();
        currentLevelMeta.LevelInitialisation();
        SetLevel();
    }
    public void NextPlushie()
    {
        var coinHandler = ServiceLocator.GetService<ICoinsHandler>();
        if(coinHandler != null)
            coinHandler.ResetCoinList();
        var connectionHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if (connectionHandler != null) connectionHandler.DeleteAllThreadLinks();

        currentLevelMeta.sewnPlushie.SetActive(false);

        LevelIncrementProcess();

        var canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        if (canvasHandler != null)
        {
            canvasHandler.startText.transform.localScale = Vector3.zero;
            canvasHandler.startText.SetActive(true);
            canvasHandler.stitchProgress.text = "0% Done";
            canvasHandler.stitchCountText.text = currentLevelMeta.noOfLinks + " OF " + currentLevelMeta.totalCorrectLinks;
        }
        
    }
    public void SetLevel()
    {
        currentLevelData.DisableAllPlushies();
        currentLevelMeta.gameObject.SetActive(true);
    }
   
    void LoadLastSavedProgress()
    {
        levelIndex = PlayerPrefs.GetInt("Level");
        plushieIndex = PlayerPrefs.GetInt("Plushie");
        currentLevelMeta = levelStructure[levelIndex].plushie[plushieIndex];
        if (currentLevelMeta != null)
        {
            int stitchedCountOfCurrentLevel =  PlayerPrefs.GetInt("StitchedCount");
            currentLevelMeta.noOfLinks = stitchedCountOfCurrentLevel;
            currentLevelMeta.noOfStitchedPart = PlayerPrefs.GetInt("StitchedPartCount");
            currentLevelMeta.Delay();
            currentLevelMeta.CheckIfStitchedBeforeCompleteScreen();
        }
    }
}
