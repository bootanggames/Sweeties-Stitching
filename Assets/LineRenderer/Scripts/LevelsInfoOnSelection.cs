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
        int levelIndex = PlayerPrefs.GetInt("Level");
        NextPage(1);
        // current page for slider on objective screen
    }

    public void NextPage(int page)
    {
        if (pageScroller != null)
        {
            //var page = pageScroller._currentPage;
            //page++;
            Debug.LogError(" " + page + " " + pageSlider._pages.Count);
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
