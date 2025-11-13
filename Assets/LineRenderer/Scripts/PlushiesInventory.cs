using TMPro;
using TS.PageSlider;
using UnityEngine;

public class PlushiesInventory : MonoBehaviour
{
    [SerializeField] PageContainer[] plushies;
    [SerializeField] TextMeshProUGUI coinUi;
    [SerializeField] TextMeshProUGUI totalPlushies;
    [SerializeField] PageScroller pageScroller;
    [SerializeField] PageSlider pageSlider;
  
    public void NextPage()
    {
        if (pageScroller != null)
        {
            var page = pageScroller._currentPage;
            page++;
            if(page < pageSlider._pages.Count)
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
