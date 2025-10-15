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
    public Transform targetCameraPoint;

    [SerializeField] GameObject[] completeConfetti;
    [SerializeField] int confettiIndex = 0;
    [SerializeField] GameObject completeStitchTextObj;
    [SerializeField] string text;
    public void MarkStitched()
    {
        IsStitched = true;
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
        if (completeConfetti == null) return;
        if (completeConfetti.Length == 0) return;
        if(confettiIndex < completeConfetti.Length) 
            completeConfetti[confettiIndex].SetActive(true);
        confettiIndex++;
        CancelInvoke("EnableConffetti");

        if (confettiIndex < completeConfetti.Length)
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
