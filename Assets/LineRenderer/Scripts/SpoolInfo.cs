using System;
using UnityEngine;
using UnityEngine.UI;

public class SpoolInfo : MonoBehaviour
{
    public int noOfStitchedDone = 0;
    public int remainigThreads;
    public int totalThreadsInSpool;
    public RectTransform undoPosition;
    public Image spoolImage;

    public void UpdateThreadProgress(float totalThreads)
    {
        //Debug.LogError(" " + totalThreads);
        totalThreadsInSpool = (int)totalThreads;
        int remainigThreads = totalThreadsInSpool - noOfStitchedDone;
        float fillBarPercent = (float)remainigThreads / totalThreads;
        //Debug.LogError(" " + remainigThreads + " " + totalThreadsInSpool + " " + noOfStitchedDone + " " + fillBarPercent);
        spoolImage.fillAmount = (float)Math.Round(fillBarPercent, 1);

    }
}
