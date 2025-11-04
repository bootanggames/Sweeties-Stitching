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
    [field: SerializeField]public bool IsStitched { get; private set; }
    public List<SewPoint> connectPoints;
    public bool head;
    public float pullForce;
    public int totalConnections;
    public int noOfConnections;

    [SerializeField] GameObject completeStitchTextObj;
    [SerializeField] GameObject wrongSequenceAlert;
    [SerializeField] string text;
    [SerializeField] List<GameObject> confettiObj;
    [SerializeField] int confettiIndex = 0;
    public List<Vector3> movedPositions;
    [SerializeField] GameObject cotton;
    [SerializeField] GameObject partWithHoles;
    [SerializeField] GameObject partWithOutHoles;

    [Header("----------------New Data------------------")]
    [SerializeField] GameObject pointPrefab;
    [SerializeField] Transform pointParent;
    [SerializeField] Transform firstPoint;
    [SerializeField] float pointsXDistance;
    [SerializeField] float maxHeightOffset;
    Vector3 prevPos;
    [SerializeField] List<Vector3> positions = new List<Vector3>();
    [SerializeField] List<SewPoint> generatedPoints;
    [SerializeField] bool dontChangeY;
    private void OnEnable()
    {
        if (completeStitchTextObj != null)
            wrongSequenceAlert = completeStitchTextObj;

        Invoke("CheckSaveProgress", 0.5f);
    }
    void CheckSaveProgress()
    {
        var gameHandler = ServiceLocator.GetService<IGameHandler>();
        if (gameHandler != null)
        {
            if (gameHandler.saveProgress)
                LoadSavedProgress();
        }
        CancelInvoke("CheckSaveProgress");
    }
    void LoadSavedProgress()
    {
        int stitched = PlayerPrefs.GetInt(partType.ToString() + "_IsStiched");
        if (stitched == 1)
        {
            IsStitched = true;
            noOfConnections = totalConnections;
            if (LevelsHandler.instance)
            {
                if (LevelsHandler.instance.currentLevelMeta)
                {

                    foreach (GameObject g in LevelsHandler.instance.currentLevelMeta.bodyParts)
                    {
                        if (partType.Equals(g.GetComponent<ObjectInfo>().partType))
                        {
                            if (partType.Equals(PlushieActiveStitchPart.neck))
                                LevelsHandler.instance.currentLevelMeta.head.transform.position = movedPosition;
                            else
                            {
                                if (partWithOutHoles == null)
                                {
                                    if (!movedPosition.Equals(Vector3.zero))
                                        g.transform.position = movedPosition;
                                }
                                else
                                {
                                    partWithHoles.SetActive(false);
                                    partWithOutHoles.SetActive(true);
                                    if (cotton) cotton.SetActive(false);
                                }

                            }
                        }
                    }
                }
            }


        }
    }
    //private void Start()
    //{
    //    SpawnPoints();

    //}
    void UpdateNewPoint(Vector3 pos , Transform point)
    {
        positions.Add(pos);
        generatedPoints.Add(point.GetComponent<SewPoint>());
        point.name = "Point " + generatedPoints.Count;
        SewPoint s = point.GetComponent<SewPoint>();
        s.ChangeText(generatedPoints.Count.ToString());
    }
    void SpawnPoints()
    {
        if (firstPoint == null) return;
        float startY = firstPoint.transform.localPosition.y;
        float startX = firstPoint.transform.localPosition.x;
        if (prevPos.Equals(Vector3.zero))
        {
            prevPos = firstPoint.transform.localPosition;
            UpdateNewPoint(prevPos, firstPoint);
        }

        Vector3 nextPointPos = Vector3.zero;

        if ((positions.Count) < totalConnections)
        {
            GameObject p = Instantiate(pointPrefab, pointParent, false);
            p.transform.SetParent(pointParent);
            p.transform.localPosition = Vector3.zero;

            float newX = prevPos.x + pointsXDistance;
          
            float endX = startX + pointsXDistance * (totalConnections - 1);
            float endY = prevPos.y;
            float t = Mathf.InverseLerp(startX, endX, newX);
            float baseHeight = Mathf.Lerp(startY, endY, t);
            float arcHeight = (1 - Mathf.Pow((2 * t) - 1, 2)) * maxHeightOffset;
            float curveY = 0;
            if (dontChangeY)
                 curveY = startY + arcHeight;
            else
                curveY = baseHeight + arcHeight;

            nextPointPos = new Vector3(newX, curveY, prevPos.z);


            if (positions.Count > 0)
            {
                if (!positions.Contains(nextPointPos))
                {
                    UpdateNewPoint(nextPointPos, p.transform);
                    p.transform.localPosition = nextPointPos;
                }
                else
                    return;
            }
            else
            {
                UpdateNewPoint(nextPointPos, p.transform);
                p.transform.localPosition = nextPointPos;
            }

            prevPos = nextPointPos;
            SpawnPoints();
        }
      
    }
 
    public void MarkStitched()
    {
        IsStitched = true;
        if (cotton)
            cotton.SetActive(false);

        PlayerPrefs.SetInt(partType.ToString()+"_IsStiched", 1);

        if (partWithHoles) partWithHoles.GetComponent<SpriteRenderer>().enabled = false;
        if (partWithOutHoles) partWithOutHoles.SetActive(true);

        PlaySound();
        ChangeText(completeStitchTextObj, text, 4);
        Invoke("EnableConffetti", 0.2f);
    }
    
    void EnableConffetti()
    {
        if (confettiIndex < connectPoints.Count)
        {
            GameObject g = GameEvents.EffectHandlerEvents.onPartCompleteEffect.Raise(connectPoints[confettiIndex].transform);
            confettiObj.Add(g);
            connectPoints[confettiIndex].gameObject.SetActive(false);
            g.SetActive(true);
            confettiIndex++;
        }
            
        CancelInvoke("EnableConffetti");

        if (confettiIndex < connectPoints.Count)
            Invoke("EnableConffetti", 0.15f);
        else
            Invoke("UpdateProgress",1.5f);
    }

    void UpdateProgress()
    {
        DisableWellDoneText();

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
                    LevelsHandler.instance.currentLevelMeta.UpdateLevelProgress(sp.sequenceType);
             
                var pointHandler = ServiceLocator.GetService<IPointConnectionHandler>();
                if (pointHandler != null)
                {
                    foreach (Connections c in pointHandler.connections)
                    {
                        Destroy(c.line.gameObject);
                    }
                    pointHandler.connections.Clear();
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
        }
       
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
        IsStitched = false;
        if (partWithOutHoles) partWithOutHoles.SetActive(false);
        if (partWithHoles)
        {
            partWithHoles.SetActive(true);
            partWithHoles.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (cotton) cotton.SetActive(true);

        foreach (SewPoint s in connectPoints)
        {
            s.connected = false;
            if(s.pointMesh) s.pointMesh.enabled = true;
            s.gameObject.SetActive(true);
            s.ChangeTextColor(Color.white);
            s.GetComponent<Collider>().enabled = true;
        }
    }
}
