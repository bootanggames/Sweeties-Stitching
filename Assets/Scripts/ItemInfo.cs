using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public string itemName;
    [field: SerializeField] public ItemStatus status { get; private set; }
    public GameObject lockImage;
    [SerializeField] Button _button;
    [field: SerializeField]public GameObject priceButton {  get; private set; }
    [SerializeField] TextMeshProUGUI itemPrice;
    IJackpotHandler jackpotHandler;
    
    public void SetItem(UIContext uiContext)
    {
        itemName = uiContext.LabelToSet;
        lockImage.GetComponent<Image>().sprite = uiContext.ImageToSet;
        CheckLockUnlockItems(uiContext.Cost);
        SetUnlock();
    }
    public void SetUnlock()
    {
        jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
        if(jackpotHandler != null)
        {
            if (jackpotHandler.earnedItems.Find(x => x.DisplayName == itemName))
            {
                status = ItemStatus.unlocked;
                lockImage.SetActive(false);
                priceButton.SetActive(false);
            }
        }
      
    }

    public void Unlock()
    {
        status = ItemStatus.unlocked;
        lockImage.SetActive(false);
        priceButton.SetActive(false);
        PlayerPrefs.SetInt(itemName + "_" + ItemStatus.unlocked.ToString(), 1);
    }

    public void CheckLockUnlockItems(int c)
    {
        itemPrice.text = c.ToString();
        if (jackpotHandler == null)
        {
            if (PlayerPrefs.GetInt(itemName + "_" + ItemStatus.unlocked.ToString()) == 1)
                status = ItemStatus.unlocked;
        }
            
        if (status.Equals(ItemStatus.locked))
        {
            lockImage.SetActive(true);
        }
        else
        {
            lockImage.SetActive(false);
            status = ItemStatus.unlocked;
        }
    }
}
