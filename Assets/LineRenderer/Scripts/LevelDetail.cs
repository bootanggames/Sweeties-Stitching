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
        if(LevelsHandler.instance)
            levelNumber.text = (id+1)+"/"+LevelsHandler.instance.levels.Count;
    }
}
