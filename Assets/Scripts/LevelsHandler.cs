using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class LevelsHandler : Singleton<LevelsHandler>, ILevelHandler
{
    [field:SerializeField] public List<LevelStructure> levelStructure {  get; private set; }
    //[field:SerializeField] public List<GameObject> levels {  get; private set; }
    [field: SerializeField] public int levelIndex { get; private set; }
    [field: SerializeField] public int plushieIndex { get; private set; }
    public LevelStructure currentLevelData { get; private set; }
    public Level_Metadata currentLevelMeta { get; private set; }
    int totalCoins;
    ICoinsHandler coinHandler;
    IThreadManager IThreadHandler;
    ICanvasUIManager canvasHandler;
    IPointConnectionHandler connectionHandler;
    ISpoolManager spoolManager;
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
        spoolManager = ServiceLocator.GetService<ISpoolManager>();
        connectionHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        canvasHandler = ServiceLocator.GetService<ICanvasUIManager>();
        IThreadHandler = ServiceLocator.GetService<IThreadManager>();
        coinHandler = ServiceLocator.GetService<ICoinsHandler>();
        if (GameHandler.instance != null)
        {
            if(GameHandler.instance.saveProgress)
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
            PlayerPrefs.SetInt("LevelUp", 1);
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
    bool once = false;
    public void NextPlushie()
    {
        if (GameHandler.instance.gameStates.Equals(GameStates.jackpotScreen)) return;
        if (once) return;
        if (connectionHandler != null) connectionHandler.DeleteAllThreadLinks();

        int rewardedCoins = currentLevelMeta.levelScriptable.levelReward;
        if (spoolManager != null)
        {
            if (currentLevelMeta.currentActiveSpoolIndex >= spoolManager.spoolList.Count)
                currentLevelMeta.currentActiveSpoolIndex = 0;
        }
           
        LevelIncrementProcess();
        if (canvasHandler != null)
        {
            canvasHandler.startText.transform.localScale = Vector3.zero;
            canvasHandler.startText.SetActive(true);
            canvasHandler.stitchProgress.text = "0% Done";
            canvasHandler.stitchCountText.text = currentLevelMeta.noOfStitchesDone + " OF " + currentLevelMeta.levelScriptable.totalStitches;
        }
        if(IThreadHandler != null) IThreadHandler.SetUndoValue(true);
        if (coinHandler != null) coinHandler.StopCoinSoundOnComplete();
        //GameEvents.GameCompleteEvents.onFinishCoinBurstAnimation.Raise();
        SoundManager.instance.StopSound(coinHandler.audioSource);
        HepticManager.instance.StopHaptics();
        DOTween.KillAll();
       once = true;
    }
    public void SwitchScreen()
    {
        int levelUp = PlayerPrefs.GetInt("LevelUp");
        if(levelUp == 1)
        {
            GameHandler.instance.Home("HomeScreen");
        }
        else
        {
            canvasHandler.EnableDisablePlushieInventoryScreen(true);
            canvasHandler.EnableDisableGameCompleteScreen(false);
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
        plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        currentLevelMeta = levelStructure[levelIndex].plushie[plushieIndex];
        if (currentLevelMeta != null)
        {
            int stitchedCountOfCurrentLevel =  PlayerPrefs.GetInt("StitchedCount");
            currentLevelMeta.noOfStitchesDone = stitchedCountOfCurrentLevel;
            currentLevelMeta.noOfStitchedPart = PlayerPrefs.GetInt("StitchedPartCount");
            currentLevelMeta.CheckIfStitchedBeforeCompleteScreen();
        }
    }
    void ChangeTextPosition(TextMeshProUGUI textMesh, AlertTextPosition pos)
    {
        textMesh.rectTransform.anchoredPosition = pos.textanchorPos;
        Rect r = new Rect();
        r.xMin = pos.minX; 
        r.xMax = pos.maxX;
        r.yMin = pos.minY;
        r.yMax = pos.maxY;
        textMesh.rectTransform.anchorMin = new Vector2(r.xMin, r.yMin);
        textMesh.rectTransform.anchorMax = new Vector2(r.xMax, r.yMax);
        textMesh.rectTransform.pivot = new Vector2(pos.pivotX, pos.pivotY);
    }
    public void ChangeText(string _text, float _fontSize, PlushieActiveStitchPart partInfo)
    {
        GameObject textObj = null;
        textObj = canvasHandler.alertTextObj;

        if (textObj != null)
        {
            TextMeshProUGUI textMesh = textObj.GetComponent<TextMeshProUGUI>();
            AlertTextPosition pos = null;
            switch (partInfo)
            {
                case PlushieActiveStitchPart.neck:
                    pos = currentLevelMeta.levelScriptable.neckTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.righteye:
                    pos = currentLevelMeta.levelScriptable.rightEyeTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.rightear:
                    pos = currentLevelMeta.levelScriptable.rightEarTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.leftear:
                    pos = currentLevelMeta.levelScriptable.leftEarTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.lefteye:
                    pos = currentLevelMeta.levelScriptable.leftEyeTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.leftarm:
                    pos = currentLevelMeta.levelScriptable.leftArmTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.leftleg:
                    pos = currentLevelMeta.levelScriptable.leftLegTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.rightleg:
                    pos = currentLevelMeta.levelScriptable.rightLegTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
                case PlushieActiveStitchPart.rightarm:
                    pos = currentLevelMeta.levelScriptable.rightArmTextPos;
                    ChangeTextPosition(textMesh, pos);
                    break;
            }
            
            textMesh.fontSize = _fontSize;
            textMesh.text = _text;
            textObj.SetActive(true);
        }
    }
}
