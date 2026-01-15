using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public string itemName;
    [field: SerializeField] public ItemStatus status { get; private set; }
    public GameObject lockImage;
    [SerializeField] Button _button;
    [SerializeField] GameObject priceButton;
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
                Unlock();
            }
        }
      
    }

    public void Unlock()
    {
        status = ItemStatus.unlocked;
        lockImage.SetActive(false);
        _button.interactable = true;

        priceButton.SetActive(false);
    }
    
    public void CheckLockUnlockItems(int c)
    {
        itemPrice.text = c.ToString();

        if (status.Equals(ItemStatus.locked))
        {
            lockImage.SetActive(true);
            //_button.interactable = false;
        }
        else
        {
            _button.interactable = true;
            lockImage.SetActive(false);
            status = ItemStatus.unlocked;
        }
    }
}
