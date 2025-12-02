using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public PlushieActiveStitchPart partType;

    public PartConnectedTo partConnectedTo;
    public Vector3 originalRotation;
    public Vector3 movedPosition;
    public Vector3 startPosition;
    public bool moveable = false;
    public bool shouldBeChild = false;
    public List<SewPoint> connectPoints;
    public bool head;
    public float pullForce;
    public int totalConnections;

    [SerializeField] GameObject completeStitchTextObj;
    [SerializeField] GameObject wrongSequenceAlert;
    [SerializeField] string text;
    [SerializeField] List<GameObject> confettiObj;
    [SerializeField] int confettiIndex = 0;
    public PlushiePartStitchData stitchData;
    [SerializeField] GameObject cotton;
    [SerializeField] GameObject partWithHoles;
    public GameObject partWithOutHoles;
    public CleanStitch c_Stitch;
    [SerializeField] bool enableConnection;
    List<GameObject> coinsObj = new List<GameObject>();
    //[Header("----------------New Data------------------")]
    //[SerializeField] GameObject pointPrefab;
    //[SerializeField] Transform pointParent;
    //[SerializeField] Transform firstPoint;
    //[SerializeField] float pointsXDistance;
    //[SerializeField] float maxHeightOffset;
    //Vector3 prevPos;
    //[SerializeField] List<Vector3> positions = new List<Vector3>();
    //[SerializeField] List<SewPoint> generatedPoints;
    //[SerializeField] bool dontChangeY;
    private void Start()
    {
        c_Stitch = GetComponent<CleanStitch>();
    }
    private void OnEnable()
    {
        if (completeStitchTextObj != null)
            wrongSequenceAlert = completeStitchTextObj;
        stitchData = new PlushiePartStitchData();
        LoadSavedData();
        //Invoke(nameof(LoadSavedData), 0.25f);
    }
 
    void LoadSavedData()
    {
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler != null)
        {
            if (gameHandler.saveProgress)
            {
                if (SaveDataUsingJson.instance)
                    stitchData = SaveDataUsingJson.instance.LoadData<PlushiePartStitchData>(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + partType);
                if(stitchData == null)
                    stitchData= new PlushiePartStitchData();
                if (stitchData.movedPositions.Count > 0)
                {
                    if (moveable)
                    {
                        if (!head)
                            this.transform.position = stitchData.movedPositions[stitchData.movedPositions.Count - 1];

                        else
                            LevelsHandler.instance.currentLevelMeta.head.transform.position = stitchData.movedPositions[stitchData.movedPositions.Count - 1];
                    }
                }
                if (stitchData.IsStitched)
                {
                    ChangePartsState(false);
                    if (head)
                        LevelsHandler.instance.currentLevelMeta.head.transform.position = movedPosition;

                    if (!moveable)
                    {
                        if (!partType.Equals(PlushieActiveStitchPart.lefteye) && !partType.Equals(PlushieActiveStitchPart.righteye))
                        {
                            for (int i = 0; i < stitchData.movedPositions.Count; i++)
                            {
                                GameObject crissCross = Instantiate(LevelsHandler.instance.currentLevelMeta.levelScriptable.stitchObj, connectPoints[i].transform);
                                crissCross.GetComponent<SpriteRenderer>().color = LevelsHandler.instance.currentLevelMeta.levelScriptable.threadColor;
                                crissCross.transform.localPosition = Vector3.zero;
                                if (partType.Equals(PlushieActiveStitchPart.leftarm) || partType.Equals(PlushieActiveStitchPart.rightarm))
                                    crissCross.transform.localEulerAngles = new Vector3(0, 0, 0);
                                else
                                    crissCross.transform.localEulerAngles = new Vector3(0, 0, 90);
                                crissCross.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                                crissCross.SetActive(true);
                                crissCross.transform.SetParent(connectPoints[i].transform.parent);
                                if (!LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Contains(crissCross))
                                    LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Add(crissCross);
                            }
                        }
                    }
                    if (partType.Equals(PlushieActiveStitchPart.lefteye) || partType.Equals(PlushieActiveStitchPart.righteye))
                    {
                        if (moveable)
                        {
                            this.transform.position = movedPosition;
                            GameObject crissCross = Instantiate(LevelsHandler.instance.currentLevelMeta.levelScriptable.crissCrossObjForEyes, this.transform);
                            crissCross.GetComponent<SpriteRenderer>().color = LevelsHandler.instance.currentLevelMeta.levelScriptable.threadColor;
                            crissCross.transform.localScale = new Vector3(3, 3, 3);

                            //crissCross.transform.SetParent(null);
                            crissCross.transform.localEulerAngles = Vector3.zero;
                            Vector3 pos = crissCross.transform.localPosition;
                            pos.z = 1;
                            crissCross.transform.localPosition = pos;
                            if (!LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Contains(crissCross))
                                LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Add(crissCross);
                        }

                    }
                }
               
            }
            else
            {
                if (SaveDataUsingJson.instance)
                    SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + partType, stitchData);
            }
        }
        CancelInvoke(nameof(LoadSavedData));
    }
    public void PartPositioning(GameObject obj, Vector3 position)
    {
        if (!position.Equals(Vector3.zero))
            obj.transform.position = position;
    }
    public void ChangePartsState(bool enable)
    {
        if (partWithHoles)
        {
            partWithHoles.SetActive(enable);
            partWithHoles.GetComponent<SpriteRenderer>().enabled = enable;
        }
        if (cotton) cotton.SetActive(enable);
        if(partWithOutHoles) partWithOutHoles.SetActive(!enable);
    }
   

    public void IncementConnection()
    {
        stitchData.noOfConnections++;
        SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + partType, stitchData);
    }
    public void DecementConnection()
    {
        stitchData.noOfConnections--;
        SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + partType, stitchData);
    }
    public void MarkStitched()
    {
        stitchData.IsStitched = true;

        SaveDataUsingJson.instance.SaveData(LevelsHandler.instance.currentLevelMeta.levelScriptable.levelName + "_" + partType, stitchData);

        if (cotton) cotton.SetActive(false);

        PlayerPrefs.SetInt(partType.ToString()+"_IsStiched", 1);

        if (partWithHoles) partWithHoles.GetComponent<SpriteRenderer>().enabled = false;
        if (partWithOutHoles) partWithOutHoles.SetActive(true);
        if(c_Stitch) c_Stitch.UnParentPoints();

        PlaySound();
        ChangeText(completeStitchTextObj, text, 4);
        var pointHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if (pointHandler != null)
        {
            foreach (Connections c in pointHandler.connections)
            {
                Destroy(c.line.gameObject);
            }
            pointHandler.connections.Clear();
        }
        if (head)
        {
            if (moveable)
            {
                LevelsHandler.instance.currentLevelMeta.head.transform.localPosition = movedPosition;
                //Debug.LogError(" " + movedPosition+" "+ LevelsHandler.instance.currentLevelMeta.head.transform.localPosition);
                LevelsHandler.instance.currentLevelMeta.immoveablePart.GetComponent<SpriteRenderer>().enabled = false;
                LevelsHandler.instance.currentLevelMeta.bodyWihtoutHoles.SetActive(true);

            }
        }
        if (moveable)
        {
            LevelsHandler.instance.currentLevelMeta.noOfStitchedPart++;
            PlayerPrefs.SetInt("StitchedPartCount", LevelsHandler.instance.currentLevelMeta.noOfStitchedPart);
        }

        LevelsHandler.instance.currentLevelMeta.UpdateCleanThreadConnections();
        if (partType.Equals(PlushieActiveStitchPart.lefteye) || partType.Equals(PlushieActiveStitchPart.righteye))
        {
            if (moveable)
            {
                GameObject crissCross = Instantiate(LevelsHandler.instance.currentLevelMeta.levelScriptable.crissCrossObjForEyes, this.transform);
                crissCross.GetComponent<SpriteRenderer>().color = LevelsHandler.instance.currentLevelMeta.levelScriptable.threadColor;
                crissCross.transform.localScale = new Vector3(3, 3, 3);
                //crissCross.transform.SetParent(null);
                crissCross.transform.localEulerAngles = Vector3.zero;
                Vector3 pos = crissCross.transform.localPosition;
                pos.z = 1;
                crissCross.transform.localPosition = pos;
                if (!LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Contains(crissCross))
                    LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Add(crissCross);
            }
      
        }
        Invoke("EnableConffetti", 0.2f);
    }
    int index = 0;
    [SerializeField] int noOfCleanThreadConnections = 0;
    void EnableConffetti()
    {
        if (confettiIndex < connectPoints.Count)
        {
            GameObject g = GameEvents.EffectHandlerEvents.onPartCompleteEffect.Raise(connectPoints[confettiIndex].transform);
            confettiObj.Add(g);
            connectPoints[confettiIndex].gameObject.SetActive(false);
            g.SetActive(true);
            if (!moveable)
            {
                var coinHandler = ServiceLocator.GetService<ICoinsHandler>();
                if (coinHandler != null)
                {
                    coinHandler.InstantiateCoins(coinHandler.coinSpritePrefab, 1, coinsObj, connectPoints[confettiIndex].transform);
                    foreach(GameObject c in coinsObj)
                    {
                        c.transform.SetParent(this.transform);
                        GameEvents.EffectHandlerEvents.onSparkleTrailEffect.RaiseEvent(c.transform);
                    }
                    //coinHandler.SaveCoins((confettiIndex + 1));
                    Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, coinHandler.coinsGameplayTarget.position);
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                    Transform targetPos = new GameObject("Temp").transform;
                    targetPos.position = worldPos;
                    StartCoroutine(coinHandler.MoveCoins(coinsObj, targetPos, coinHandler.coinBarForGameplayScreen, coinHandler.coinMoveSpeed, Ease.Linear,0));
                    //Debug.LogError(" " + (confettiIndex + 1));
                }
                if (!partType.Equals(PlushieActiveStitchPart.lefteye) && !partType.Equals(PlushieActiveStitchPart.righteye))
                {
                    GameObject crissCross = Instantiate(LevelsHandler.instance.currentLevelMeta.levelScriptable.stitchObj, connectPoints[confettiIndex].transform);
                    crissCross.GetComponent<SpriteRenderer>().color = LevelsHandler.instance.currentLevelMeta.levelScriptable.threadColor;
                    crissCross.transform.localPosition = Vector3.zero;
                    if(partType.Equals(PlushieActiveStitchPart.leftarm) || partType.Equals(PlushieActiveStitchPart.rightarm))
                        crissCross.transform.localEulerAngles = new Vector3(0, 0, 0);
                    else
                        crissCross.transform.localEulerAngles = new Vector3(0, 0, 90);
                    crissCross.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

                    crissCross.SetActive(true);
                    crissCross.transform.SetParent(connectPoints[confettiIndex].transform.parent);
                    if (!LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Contains(crissCross))
                        LevelsHandler.instance.currentLevelMeta.crissCrossObjList.Add(crissCross);
                }
            }
         
            confettiIndex++;
        }
        if(index < noOfCleanThreadConnections)
        {
            if (enableConnection)
            {
                if(partType.Equals(PlushieActiveStitchPart.lefteye) || partType.Equals(PlushieActiveStitchPart.righteye))
                {
                    if (LevelsHandler.instance.currentLevelMeta.cleanThreadIndex < LevelsHandler.instance.currentLevelMeta.cleanConnection.Count)
                        LevelsHandler.instance.currentLevelMeta.cleanConnection[LevelsHandler.instance.currentLevelMeta.cleanThreadIndex].line.gameObject.SetActive(true);

                }

                LevelsHandler.instance.currentLevelMeta.cleanThreadIndex++;
            }
            index++;
        }
        

        CancelInvoke("EnableConffetti");

        if (confettiIndex < connectPoints.Count)
            Invoke("EnableConffetti", 0.15f);
        else
        {
            if (LevelsHandler.instance.currentLevelMeta.noOfStitchedPart.Equals(LevelsHandler.instance.currentLevelMeta.levelScriptable.totalParts))
            {
                int levelIndex = PlayerPrefs.GetInt("Level");

                int plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
                LevelsHandler.instance.UpdatePlushieInventory(levelIndex, plushieIndex);
            }
               
            Invoke("UpdateProgress", 1.5f);
        }
    }

    void UpdateProgress()
    {
        DisableWellDoneText();
        foreach(GameObject g in confettiObj)
        {
            Destroy(g);
        }
        confettiObj.Clear();
        SewPoint sp = null;

        IThreadManager threadManager = ServiceLocator.GetService<IThreadManager>();
        if (threadManager != null)
        {
            if (threadManager.detectedPoints != null && threadManager.detectedPoints.Count > 0)
            {
                sp = threadManager.detectedPoints[threadManager.detectedPoints.Count - 1].GetComponent<SewPoint>();
                sp.name = sp.sequenceType.ToString();
                GameEvents.ThreadEvents.onResetThreadInput.RaiseEvent();
                if (LevelsHandler.instance.currentLevelMeta)
                {
                    //foreach (Connections c in LevelsHandler.instance.currentLevelMeta.cleanConnection)
                    //{
                    //    Destroy(c.line.gameObject);
                    //}
                    //LevelsHandler.instance.currentLevelMeta.cleanConnection.Clear();
                    LevelsHandler.instance.currentLevelMeta.UpdateLevelProgress(sp.sequenceType);
                }


            }
        }
        CancelInvoke("UpdateProgress");
    }
    public void DisableWellDoneText()
    {
        if (completeStitchTextObj)
        {
            completeStitchTextObj.SetActive(false);
            Destroy(completeStitchTextObj);
        }
        CancelInvoke("DisableWellDoneText");
    }

    void ChangeText(GameObject textObj, string _text, float _fontSize)
    {
        if (textObj != null)
        {
            TextMeshPro textMesh = textObj.GetComponent<TextMeshPro>();
            textMesh.fontSize = _fontSize;
            textMesh.text = _text;
            textObj.SetActive(true);
        }
    }
    public void DisableWrongAlertText()
    {
        if (wrongSequenceAlert != null)
            wrongSequenceAlert.SetActive(false);
    }
    public void WrongSequenceAlertText(string _text, float _fontSize)
    {
        if (wrongSequenceAlert)
        {
            TextMeshPro textMesh = wrongSequenceAlert.GetComponent<TextMeshPro>();
            textMesh.fontSize = _fontSize;
            textMesh.text = _text;
            wrongSequenceAlert.SetActive(true);
            WrongSelectedPointSound();
        }
       
    }

    void WrongSelectedPointSound()
    {
        SoundManager.instance.ResetAudioSource();
        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.wrongStitchSound;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
    public void ResetPart()
    {
        if (moveable) this.transform.position = startPosition;

        foreach(GameObject g in confettiObj)
        {
            Destroy(g);
        }
        confettiObj.Clear();
        confettiIndex = 0;
        stitchData.IsStitched = false;
        PlayerPrefs.SetInt(partType.ToString() + "_IsStiched", 0);
        ChangePartsState(true);
        stitchData.noOfConnections = 0;
        var connectHandler = ServiceLocator.GetService<IPointConnectionHandler>();
        if (connectHandler != null)
        {
            foreach (SewPoint s in connectPoints)
            {
                s.IsConnected(false, 0, Vector3.zero, "");
                s.Selected(false);
                if (s.pointMesh)
                {
                    s.pointMesh.enabled = true;
                    s.pointMesh.material = connectHandler.originalMaterial;
                }
                s.gameObject.SetActive(true);
                s.ChangeTextColor(Color.white);
                s.GetComponent<Collider>().enabled = true;
            }
        }
            
    }
}
