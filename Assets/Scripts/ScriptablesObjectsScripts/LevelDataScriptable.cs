using UnityEngine;

[CreateAssetMenu(fileName ="LevelData",menuName ="Level/LevelData")]
public class LevelDataScriptable : ScriptableObject
{
    public string levelName;
    public int levelReward = 0;
    public int totalStitches;
    public int totalParts;
    public int totalSpoolsNeeded;
    public float plushieWidth;
    public float plushieHeight;
    public Sprite plushieSprite;
    public Color threadColor;
    public Sprite threadSpool;
    public GameObject stitchObj;
    public GameObject crissCrossObjForEyes;

    public Vector3 neckCameraPos;
    public Vector3 rightEyeCameraPos;
    public Vector3 rightEarCameraPos;
    public Vector3 leftEarCameraPos;
    public Vector3 leftEyeCameraPos;
    public Vector3 leftArmCameraPos;
    public Vector3 leftLegCameraPos;
    public Vector3 rightLegCameraPos;
    public Vector3 rightArmCameraPos;
    public Vector3 gameCompleteCameraPos;
}
