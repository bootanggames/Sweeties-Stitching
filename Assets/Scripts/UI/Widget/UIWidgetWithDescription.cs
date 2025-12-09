using UnityEngine;
using TMPro;

public class UIWidgetWithDescription : UIWidget
{
    [SerializeField] protected TextMeshProUGUI _descriptionText;

    public virtual void SetDescription(string description)
    {
        _descriptionText.text = description;
    }
}
