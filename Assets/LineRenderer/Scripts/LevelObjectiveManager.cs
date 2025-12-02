using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
        //LevelsHandler.instance.SetLevelLockState(0, 0, 1);

        for (int i = 0; i < levels.levelPage.Count; i++)
        {
            for (int j=0; j< levels.levelPage[i].levelDetail.Count;j++)
            {
                LevelDetail levelD = levels.levelPage[i].levelDetail[j].levelObject.GetComponent<LevelDetail>();
                int lockState = PlayerPrefs.GetInt("Level_" + i + "Plushie_" + j);
                if (lockState == 1)
                    levelD.locked = false;
                else
                    levelD.locked = true;
                if (levelD.locked)
                    levelD.lockedImage.SetActive(true);
                else
                    levelD.lockedImage.SetActive(false);
            }
        }
    }
}
