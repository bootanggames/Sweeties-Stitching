using TMPro;
using UnityEngine;

public class LevelObjectiveManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalBodyPartsToStitch;
    [SerializeField] TextMeshProUGUI totalStitches;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI threadSpoolCount;
    [SerializeField] LevelsInfoOnSelection levels;
   
    private void Start()
    {

        UpdateObjectiveScreenOfCurrentLevel();
        LockUnLock();
  
        int levelUp = PlayerPrefs.GetInt("LevelUp");
        if (levelUp == 0)
            levels.NextLevelPage();
            //Invoke(nameof(NextPage), 0.5f);
    }
    void NextPage()
    {
        //levels.GoToNextLevelPage();
    }
    public void UpdateObjectiveScreenOfCurrentLevel()
    {

        int levelIndex = PlayerPrefs.GetInt("Level");
        int plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        Level_Metadata levelData = LevelsHandler.instance.levelStructure[levelIndex].plushie[plushieIndex];
        totalStitches.text = levelData.levelScriptable.totalStitches.ToString();
        totalBodyPartsToStitch.text = levelData.levelScriptable.totalParts.ToString();
        UpdatePlushie(levelIndex, plushieIndex);
        threadSpoolCount.text = levelData.levelScriptable.totalSpoolsNeeded.ToString()+"X";
        int c = PlayerPrefs.GetInt("Coins");
        coinText.text = c.ToString();
    }

    void UpdatePlushie(int levelIndex, int detailIndex)
    {
        for (int i = 0; i < levels.levelPage.Count; i++)
        {
            foreach (LevelSelectionObject g in levels.levelPage[i].levelDetail)
            {
                g.plushieObject.SetActive(false);
            }
        }
        levels.levelPage[levelIndex].levelDetail[detailIndex].plushieObject.SetActive(true);
    }

    void LockUnLock()
    {
        for (int i = 0; i < levels.levelPage.Count; i++)
        {
            for (int j=0; j< levels.levelPage[i].levelDetail.Count;j++)
            {
                LevelDetail levelD = levels.levelPage[i].levelDetail[j].levelObject.GetComponent<LevelDetail>();
                levelD.CheckLevelLockeUnlocked(i, j);
            }
        }
    }
}
