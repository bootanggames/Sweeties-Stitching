using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField] RectTransform itemRect;
    IRoomdecorStore _store;
    [SerializeField] string ItemName;
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
        if (!_store.changeItem) return;
        foreach(StoreItemData sd in _store.itemSpriteData)
        {
            if (sd.ItemName.Equals(ItemName))
            {
                Image itemImage = this.GetComponent<Image>();
                int r = Random.Range(0, sd._itemSprites.itemSprite.Length);
                Sprite sprite = sd._itemSprites.itemSprite[r];
                _store.ChangeItemSprite(itemImage, sprite);
            }
        }
      
        Debug.Log("onPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
    }
}
