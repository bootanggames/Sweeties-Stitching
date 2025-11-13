using System.Collections.Generic;
using UnityEngine;

public class MainMenuLevelsHandler : MonoBehaviour
{
    [SerializeField] List<LevelObjectivePageDetail> levelPage;
    private void Start()
    {
        LockUnLock();
    }
    void LockUnLock()
    {
        PlayerPrefs.SetInt("Level_" + 0 + "Plushie_" + 0, 1);

        for (int i = 0; i < levelPage.Count; i++)
        {
            for (int j = 0; j < levelPage[i].levelDetail.Count; j++)
            {
                LevelDetail levelD = levelPage[i].levelDetail[j].levelObject.GetComponent<LevelDetail>();
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
