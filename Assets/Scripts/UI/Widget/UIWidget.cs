using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIWidget : MonoBehaviour
{
    [Header("Widget Container")]

    [SerializeField] private GameObject _widgetContainer;
    
    [Header("UI Elements")]
    
    [SerializeField] protected Image _imageComponent;
    [SerializeField] protected Outline imgOutline;
    [SerializeField] protected TextMeshProUGUI _labelComponent;

    protected UIContext _uiContext;
    [field:SerializeField] public ItemInfo _itemInfo {  get; private set; }
    public virtual void SetContext(UIContext uiContext)
    {
        _uiContext = uiContext;
        
        SetImage(uiContext.ImageToSet);
        SetLabel(uiContext.LabelToSet);
        SetImageColor(uiContext.Color);
        if (_itemInfo) _itemInfo.SetItem(uiContext);//----

        IWidgetComponent[] widgets = GetComponentsInChildren<IWidgetComponent>();

        foreach (IWidgetComponent component in widgets)
        {
            component.SetUIContext(_uiContext);
        }
    }
    public int GetContextID()
    {
        return _uiContext.ID;
    }
    public void SetActive(bool status)
    {
        if (_widgetContainer == null)
        {
            gameObject.SetActive(status);
        }
        else
        {
            _widgetContainer.SetActive(status);
        }
    }

    public void SetImageColor(Color color)
    {
        if (_imageComponent == null || color == Color.clear)
            return;

        _imageComponent.color = color;
    }

    public void SetImage(Sprite sprite)
    {
        if (sprite == null || _imageComponent == null)
            return;
        
        _imageComponent.sprite = sprite;
        _imageComponent.gameObject.SetActive(true);
    }

    public void SetLabel(string labelText)
    {
        if (string.IsNullOrEmpty(labelText) || _labelComponent == null)
            return;
        
        _labelComponent.text = labelText;
    }

    public void EnableDisableOutline(bool val)
    {
        //if (_imageComponent.GetComponent<Outline>())
        imgOutline.enabled = val;
    }
    public int GetItemPrice()
    {
        if(_uiContext != null)
            return _uiContext.Cost;
        else return 0;
    }
    public void SetItemForPurchase()
    {
        _itemInfo.CheckLockUnlockItems(GetItemPrice());
    }
}