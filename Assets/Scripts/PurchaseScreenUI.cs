using UnityEngine;

public class PurchaseScreenUI : MonoBehaviour
{
    [SerializeField] GameObject unlockedItemTextObj;
    [SerializeField] GameObject rightArrow;
    [SerializeField] GameObject leftArrow;
    private void OnEnable()
    {
        Invoke(nameof(DisableText), 2.5f);
    }

    void DisableText()
    {
        unlockedItemTextObj.SetActive(false);
        CancelInvoke(nameof(DisableText));
    }
}
