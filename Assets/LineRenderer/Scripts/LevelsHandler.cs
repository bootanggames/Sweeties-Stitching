using System.Collections.Generic;
using UnityEngine;

public class LevelsHandler : Singleton<LevelsHandler>, ILevelHandler
{
    [field:SerializeField] public List<GameObject> levels {  get; private set; }
    [field: SerializeField] public int levelIndex { get; private set; }
    public override void SingletonAwake()
    {
        base.SingletonAwake();
        RegisterService();
        SetNextLevel();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
        UnRegisterService();
    }
    //private void Start()
    //{
    //    if(levelIndex == 1)
    //        GameEvents.PointConnectionHandlerEvents.onSettingPlushieLevel2.RaiseEvent(true);
    //    else
    //        GameEvents.PointConnectionHandlerEvents.onSettingPlushieLevel2.RaiseEvent(false);

    //}
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
    public void SetNextLevel()
    {
        levelIndex = PlayerPrefs.GetInt("Level");
        foreach(GameObject g in levels)
        {
            g.SetActive(false);
        }
        levels[levelIndex].SetActive(true);
    }
}
