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

        SetLevel();
        currentLevelMeta = levels[levelIndex].GetComponent<Level_Metadata>();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        UnRegisterService();
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

        if (levelIndex >= (levels.Count - 1))
            levelIndex = 0;

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
        levels[levelIndex].SetActive(true);
    }
   
}
