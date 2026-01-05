using TMPro;
using UnityEngine;

public class LevelObjectiveManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalBodyPartsToStitch;
    [SerializeField] TextMeshProUGUI totalStitches;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] LevelsInfoOnSelection levels;
    private void Start()
    {
        UpdateTotalStitchesOfCurrentLevel();
        LockUnLock();
        int c = PlayerPrefs.GetInt("Coins");
        coinText.text = c.ToString();
        int levelUp = PlayerPrefs.GetInt("LevelUp");
        if (levelUp == 0)
            Invoke(nameof(NextPage), 0.5f);
    }
    void NextPage()
    {
        levels.GoToNextLevelPage();
    }
    public void UpdateTotalStitchesOfCurrentLevel()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        int plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        Level_Metadata levelData = LevelsHandler.instance.levelStructure[levelIndex].plushie[plushieIndex];
        totalStitches.text = levelData.levelScriptable.totalStitches.ToString();
        totalBodyPartsToStitch.text = levelData.levelScriptable.totalParts.ToString();
        UpdatePlushie(levelIndex, plushieIndex);
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
