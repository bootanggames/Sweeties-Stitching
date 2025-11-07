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
    private void Start()
    {
        int index = PlayerPrefs.GetInt("Level");
        UpdateTotalStitchesOfCurrentLevel(index);
        UpdatePlushie(index);
        LockUnLock();
    }
    public void UpdateTotalStitchesOfCurrentLevel(int id)
    {
        Level_Metadata levelData = LevelsHandler.instance.levels[id].GetComponent<Level_Metadata>();
        totalStitches.text = levelData.totalCorrectLinks.ToString();
        totalBodyPartsToStitch.text = levelData.totalStitchedPart.ToString();
        UpdatePlushie(id);
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
        int index = PlayerPrefs.GetInt("Level");
        int count = LevelsHandler.instance.levels.Count;
        foreach(GameObject g in levelsDetailObject)
        {
            LevelDetail ld = g.GetComponent<LevelDetail>();
            ld.locked = true;
            ld.lockedImage.SetActive(true);
        }
        for(int i=0;i<levelsDetailObject.Count;i++)
        {
            if(i <= index)
            {
                levelsDetailObject[i].GetComponent<LevelDetail>().locked = false;
                levelsDetailObject[i].GetComponent<LevelDetail>().lockedImage.SetActive(false);
            }
        }
    }
}
