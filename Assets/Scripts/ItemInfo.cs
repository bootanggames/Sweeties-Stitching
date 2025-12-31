using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [field: SerializeField] public ItemStatus status { get; private set; }
    [SerializeField] GameObject lockImage;
    [SerializeField] Button _button;
    [SerializeField] GameObject priceButton;
    [SerializeField] TextMeshProUGUI itemPrice;
    IJackpotHandler jackpotHandler;
    public void SetUnlock(string i_Name)
    {
        jackpotHandler = ServiceLocator.GetService<IJackpotHandler>();
        if (jackpotHandler.earnedItems.Find(x=>x.DisplayName == i_Name))
        {
            status = ItemStatus.unlocked;
            lockImage.SetActive(false);
            _button.interactable = true;

            priceButton.SetActive(false);
        }
    }
    public void CheckLockUnlockItems(int c)
    {
        itemPrice.text = c.ToString();

        if (status.Equals(ItemStatus.locked))
        {
            lockImage.SetActive(true);
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
            lockImage.SetActive(false);
        }
    }
}
