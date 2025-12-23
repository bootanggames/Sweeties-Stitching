using Mkey;
using MkeyFW;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SlotControl : MonoBehaviour
{
    [SerializeField]
    private SlotController slot;
    [SerializeField]
    private LinesController linesController;
    [SerializeField]
    private Button spinButton;
    public int SelectedLinesCount
    {
        get; private set;
    }
    public Action<int, bool> ChangeSelectedLinesEvent;
    public Action<int> ChangeFreeSpinsEvent;
    private HoldFeature hold;
    public HoldFeature Hold { get { return hold; } }
    public bool UseHold
    {
        get { return (hold && hold.enabled && hold.gameObject.activeSelf); }
    }
    public int FreeSpins
    {
        get; private set;
    }
    public bool HasFreeSpin
    {
        get { return FreeSpins > 0; }
    }
    public bool Auto { get; private set; }

    public int AutoSpinsCounter;
    public void Spin_Click()
    {
        slot.SpinPress();
    }
    public bool AnyLineSelected
    {
        get { return SelectedLinesCount > 0; }
    }
    private bool autoPlayFreeSpins = true;
    public bool AutoPlayFreeSpins
    {
        get { return autoPlayFreeSpins; }
    }

    public void AddFreeSpins(int count)
    {
        SetFreeSpinsCount(FreeSpins + count);
    }
    public void SetFreeSpinsCount(int count)
    {
        count = Mathf.Max(0, count);
        bool changed = (FreeSpins != count);
        FreeSpins = count;
        if (changed) ChangeFreeSpinsEvent?.Invoke(FreeSpins);
    }
    public void SetControlActivity(bool activity, bool startButtonAcivity)
    {
       
        if (spinButton) spinButton.interactable = startButtonAcivity;
        if (linesController) { linesController.SetControlActivity(activity); }

    }
    internal void AddSelectedLinesCount(int count, bool burn)
    {
        SetSelectedLinesCount(SelectedLinesCount + count, burn);
    }
    internal void SetSelectedLinesCount(int count, bool burn)
    {
        count = Mathf.Max(1, count);
        count = Mathf.Min(linesController.LinesCount, count);

        bool changed = (SelectedLinesCount != count);
        SelectedLinesCount = count;
        if (changed)
        {
            ChangeSelectedLinesEvent?.Invoke(count, burn);
        }
    }
}
