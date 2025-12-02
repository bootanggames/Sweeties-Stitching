using UnityEngine;

[CreateAssetMenu(fileName ="LevelData",menuName ="Level/LevelData")]
public class LevelDataScriptable : ScriptableObject
{
    public string levelName;
    public int levelReward = 0;
    public int totalStitches;
    public int totalParts;
    public float plushieWidth;
    public float plushieHeight;
    public Sprite plushieSprite;
    public Color threadColor;
    public Sprite threadSpool;
    public GameObject stitchObj;
    public GameObject crissCrossObjForEyes;
}
