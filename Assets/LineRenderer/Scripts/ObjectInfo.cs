using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public PlushieActiveStitchPart partType;

    public PartConnectedTo partConnectedTo;
    public Vector3 originalRotation;
    public Vector3 movedPosition;
    public bool moveable = false;
    public bool shouldBeChild = false;
    [field: SerializeField]public bool IsStitched { get; private set; }
    public List<SewPoint> connectPoints;
    public bool head;
    public float pullForce;
    public int totalConnections;
    public int noOfConnections;

    [SerializeField] GameObject completeStitchTextObj;
    [SerializeField] string text;
    [SerializeField] List<GameObject> confettiObj;
    [SerializeField] int confettiIndex = 0;
    public List<Vector3> movedPositions;
    [SerializeField] GameObject cotton;

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
        if(cotton)
            cotton.SetActive(false);
        PlaySound();
        if (completeStitchTextObj)
        {
            completeStitchTextObj.GetComponent<TextMeshPro>().text = text;
            completeStitchTextObj.SetActive(true);
        }
        Invoke("EnableConffetti", 0.2f);
    }
    void EnableConffetti()
    {
        if (confettiIndex < connectPoints.Count)
        {
            GameObject g = GameEvents.EffectHandlerEvents.onPartCompleteEffect.Raise(connectPoints[confettiIndex].transform);
            confettiObj.Add(g);
            g.SetActive(true);
            confettiIndex++;
        }
            
        CancelInvoke("EnableConffetti");

        if (confettiIndex < connectPoints.Count)
        {
            Invoke("EnableConffetti", 0.15f);
        }
        else
        {
            Invoke("UpdateProgress",1);
        }
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

    public void ResetStitched()
    {
        IsStitched = false;
    }
    void PlaySound()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
}
