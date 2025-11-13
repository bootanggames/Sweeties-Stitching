using TMPro;
using UnityEngine;

public class LevelDetail : MonoBehaviour
{
    public int id;
    public string plushieName;
    public GameObject plushieImage;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI levelNumber;
    public GameObject lockedImage;
    public bool locked;

    private void Start()
    {
        if (LevelsHandler.instance)
        {
            int levelData = PlayerPrefs.GetInt("Level");
            levelNumber.text = (id + 1) + "/" + LevelsHandler.instance.levelStructure[levelData].plushie.Length;
        }
        //levelNumber.text = (id+1)+"/"+LevelsHandler.instance.levels.Count;
    }
}
