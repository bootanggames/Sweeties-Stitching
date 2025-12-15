using TMPro;
using TS.PageSlider;
using UnityEngine;
using static Unity.VisualScripting.Member;

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
        int c = PlayerPrefs.GetInt("Coins");
        coinUi.text = c.ToString();
        RegisterService();
        if (AudiosSourceContainer.instance)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.homeScreen);
            SoundManager.instance.PlaySound(AudiosSourceContainer.instance.plushieInventoryScreen, SoundManager.instance.audioClips.plushieInventoryScreenBgSound, true, false, 1.0f, true);
        }
            
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void BackButton()
    {
        if (AudiosSourceContainer.instance)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.plushieInventoryScreen);

            if (AudiosSourceContainer.instance.homeScreen)
                SoundManager.instance.PlaySound(AudiosSourceContainer.instance.homeScreen, SoundManager.instance.audioClips.bgMusic, true, false, 1.0f, true);
        }

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
