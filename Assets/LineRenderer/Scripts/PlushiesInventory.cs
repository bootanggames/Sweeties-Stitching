using TMPro;
using TS.PageSlider;
using UnityEngine;

public class PlushiesInventory : MonoBehaviour,IPlushieInventory
{
    [SerializeField] PageContainer[] plushies;
    [SerializeField] TextMeshProUGUI coinUi;
    [field:SerializeField] public TextMeshProUGUI totalPlushies {  get; private set; }
    [SerializeField] PageScroller pageScroller;
    [SerializeField] PageSlider pageSlider;
    [field: SerializeField] public int noOfPlushieEnabled {  get; private set; }

    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void NoPlushieIncrement(int c)
    {
        noOfPlushieEnabled = c;
        totalPlushies.text = noOfPlushieEnabled.ToString();

    }
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

    public void RegisterService()
    {
        ServiceLocator.RegisterService<IPlushieInventory>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IPlushieInventory>(this);
    }
}
