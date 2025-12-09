using System.Collections.Generic;
using TS.PageSlider;
using UnityEngine;

public class LevelsInfoOnSelection : MonoBehaviour
{
    public List<LevelObjectivePageDetail> levelPage;
    [SerializeField] PageScroller pageScroller;
    [SerializeField] PageSlider pageSlider;
    private void Start()
    {
        int levelUp = PlayerPrefs.GetInt("LevelUp");
        if(levelUp == 0)
            Invoke(nameof(GoToNextLevelPage), 0.5f);
    }
    void GoToNextLevelPage()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        //Debug.LogError(" " + levelIndex);
        NextPage(levelIndex);
        CancelInvoke(nameof(GoToNextLevelPage));
    }
    public void NextPage(int page)
    {
        if (pageScroller != null)
        {
            //var page = pageScroller._currentPage;
            //page++;
            if (page < pageSlider._pages.Count)
                pageScroller.ScrollToPage(page);
            else
                page = pageSlider._pages.Count - 1;
        }
    }
    public void PrevPage()
    {
        if (pageScroller != null)
        {
            var page = pageScroller._currentPage;
            page--;
            if (page >= 0)
                pageScroller.ScrollToPage(page);
            else
                page = 0;
        }
    }
}
