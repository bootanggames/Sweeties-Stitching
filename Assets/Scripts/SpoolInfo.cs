using System;
using System.Net.Mail;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpoolInfo : MonoBehaviour
{
    public SpoolData _spoolData;
    public RectTransform undoPosition;
    public Image spoolImage;
   
    public void UpdateThreadProgress(int totalThreads)
    {
        _spoolData.totalThreadsInSpool = totalThreads;
        int remainigThreads = _spoolData.totalThreadsInSpool - _spoolData.noOfStitchedDone;
        float fillBarPercent = (float)remainigThreads / totalThreads;
        //Debug.LogError(" " + fillBarPercent);
        spoolImage.fillAmount = fillBarPercent;
        //spoolImage.fillAmount = (float)Math.Round(fillBarPercent, 1);
       
    }
}
