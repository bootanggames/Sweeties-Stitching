using System.Collections.Generic;
using UnityEngine;

public class MainMenuLevelsHandler : MonoBehaviour
{
    [SerializeField] LevelsInfoOnSelection levels;

    private void Start()
    {
        Time.timeScale = 1;

        LockUnLock();
    }
    void LockUnLock()
    {
        PlayerPrefs.SetInt("Level_" + 0 + "Plushie_" + 0, 1);

        for (int i = 0; i < levels.levelPage.Count; i++)
        {
            for (int j = 0; j < levels.levelPage[i].levelDetail.Count; j++)
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
