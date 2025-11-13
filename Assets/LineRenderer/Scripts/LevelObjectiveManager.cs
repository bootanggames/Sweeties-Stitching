using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class LevelObjectiveManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalBodyPartsToStitch;
    [SerializeField] TextMeshProUGUI totalStitches;
    [SerializeField] List<GameObject> levelsDetailObject;
    [SerializeField] List<GameObject> plushies;
    [SerializeField] List<LevelObjectivePageDetail> levelPage;
    private void Start()
    {
        UpdateTotalStitchesOfCurrentLevel();
        LockUnLock();
    }
    public void UpdateTotalStitchesOfCurrentLevel()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        int plushieIndex = PlayerPrefs.GetInt("Plushie");
        Level_Metadata levelData = LevelsHandler.instance.levelStructure[levelIndex].plushie[plushieIndex];
        totalStitches.text = levelData.totalCorrectLinks.ToString();
        totalBodyPartsToStitch.text = levelData.totalStitchedPart.ToString();
        UpdatePlushie(plushieIndex);
    }

    void UpdatePlushie(int index)
    {
        foreach (GameObject g in plushies)
        {
            g.SetActive(false);
        }
        plushies[index].SetActive(true);
    }

    void LockUnLock()
    {
        int level = PlayerPrefs.GetInt("Level");
        int index = PlayerPrefs.GetInt("Plushie");
       
        foreach(GameObject g in levelsDetailObject)
        {
            LevelDetail ld = g.GetComponent<LevelDetail>();
            ld.locked = true;
            ld.lockedImage.SetActive(true);
        }

        for (int i = 0; i < levelsDetailObject.Count; i++)
        {
            if (i <= index)
            {
                levelsDetailObject[i].GetComponent<LevelDetail>().locked = false;
                levelsDetailObject[i].GetComponent<LevelDetail>().lockedImage.SetActive(false);
            }
        }
    }
}
