using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public PlushieActiveStitchPart partType;

    public PartConnectedTo partConnectedTo;
    public Vector3 originalRotation;
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
    [SerializeField] float pointsYDistance;
    Vector3 prevPos;
    [SerializeField] List<Vector3> positions = new List<Vector3>();
    [SerializeField] List<SewPoint> generatedPoints;
    [SerializeField] bool changeLastPointHeight;
    int index = 1;
    [SerializeField]Vector3 pos;
    //private void Start()
    //{
    //    SpawnPoints();
        
    //}
    void UpdateNewPoint(Vector3 pos , Transform point)
    {
        positions.Add(pos);
        generatedPoints.Add(point.GetComponent<SewPoint>());
        point.name = "Point " + generatedPoints.Count;
    }
    void SpawnPoints()
    {
        if (firstPoint == null) return;
        int totalCount = totalConnections;
        int midIndex = totalConnections / 2;
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

            if (index > 0 && index <= (midIndex - 1))
            {
                //if(prevPos.y < 0)
                    nextPointPos = new Vector3(prevPos.x + pointsXDistance, prevPos.y - pointsYDistance, prevPos.z);
                //else
                //    nextPointPos = new Vector3(prevPos.x + pointsXDistance, prevPos.y + pointsYDistance, prevPos.z);
            }
            else if(index > midIndex)
            {
                //if (prevPos.y < 0)
                    nextPointPos = new Vector3(prevPos.x + pointsXDistance, prevPos.y + pointsYDistance, prevPos.z);
                //else
                //    nextPointPos = new Vector3(prevPos.x + pointsXDistance, prevPos.y - pointsYDistance, prevPos.z);
            }
            else
                nextPointPos = new Vector3(prevPos.x + pointsXDistance, prevPos.y, prevPos.z);

            if (positions.Count > 0)
            {
                if (!positions.Contains(nextPointPos))
                {
                    UpdateNewPoint(nextPointPos, p.transform);
                    p.transform.localPosition = nextPointPos;
                    index++;
                }
                else
                    return;
            }
            else
            {
                UpdateNewPoint(nextPointPos, p.transform);
                p.transform.localPosition = nextPointPos;
                index++;
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
                var threadHandler = ServiceLocator.GetService<IThreadManager>();
                if (threadHandler != null)
                    threadHandler.detectedPoints.Clear();

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
