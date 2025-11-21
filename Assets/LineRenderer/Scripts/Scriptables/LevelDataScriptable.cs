using UnityEngine;

[CreateAssetMenu(fileName ="LevelData",menuName ="Level/LevelData")]
public class LevelDataScriptable : ScriptableObject
{
    public string levelName;
    public int levelReward = 0;
    public int totalStitches;
    public int noOfStitchesDone;
    public int totalParts;
    public int noOfStitchedPart;
    public float plushieWidth;
    public float plushieHeight;
    public Sprite plushieSprite;
    public Color threadColor;
    public Sprite threadSpool;
}
