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
    public int totalPlushies;
    private void Start()
    {
        int levelData = PlayerPrefs.GetInt("Level");
        levelNumber.text = (id + 1) + "/" + totalPlushies;
    }
}
