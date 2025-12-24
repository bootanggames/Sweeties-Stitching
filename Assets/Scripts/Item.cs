using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField] MyRoomItemData m_ItemData;
    [SerializeField] RectTransform itemRect;
    IRoomdecorStore _store;
    [SerializeField] string ItemName;
    [SerializeField] RoomItem roomItem;
    [SerializeField] bool moveable = false;
    private void Start()
    {
        _store = ServiceLocator.GetService<IRoomdecorStore>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (_store == null) return;
        if (_store.repositionItem)
        {
            if (moveable)
            {
                itemRect.anchoredPosition += eventData.delta / _store.canvas.scaleFactor;
                m_ItemData.posX = itemRect.anchoredPosition.x;
                m_ItemData.posY = itemRect.anchoredPosition.y;
                SaveDataUsingJson.instance.SaveData(ItemName + "ItemPosition", m_ItemData, "MyRoom");
            }

        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_store == null) return;
        if (_store.repositionItem) return;
        //if (!_store.changeItem) return;
        //_store.EnableDisableMyRoomScreen(false);
        //_store.EnableDisableChangeRoomUiParent(false);
        //_store.EnableDisableMItemsScreen(true);
        //GameEvents.UIEvents.ShowDecorItemsInventory.Raise(roomItem._decorItemType);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
    }
}
