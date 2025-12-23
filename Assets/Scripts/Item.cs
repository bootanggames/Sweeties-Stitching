using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField] RectTransform itemRect;
    IRoomdecorStore _store;
    [SerializeField] string ItemName;
    [SerializeField] RoomItem roomItem;
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
        if(_store.repositionItem)
            itemRect.anchoredPosition += eventData.delta / _store.canvas.scaleFactor;
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
