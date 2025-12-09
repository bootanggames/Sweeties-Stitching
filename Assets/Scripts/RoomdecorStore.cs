using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomdecorStore : MonoBehaviour, IRoomdecorStore
{
    [field: SerializeField] public Canvas canvas { get; private set; }
    [field: SerializeField] public bool repositionItem { get; private set; }
    [field: SerializeField] public bool changeItem { get; private set; }
    [field: SerializeField] public List<StoreItemData> itemSpriteData { get; private set; }

    private void OnEnable()
    {
        RegisterService();
    }

    private void OnDisable()
    {
        UnRegisterService();
    }

    public void ChangeItemSprite(Image item, Sprite sprite)
    {
        item.sprite = sprite;
    }

    public void RepositionItem(bool val)
    {
        repositionItem = val;
        GameEvents.RoomDecorEvents.SetRoomDecorPermissionStatus.Raise(!val);
    }

    public void ChangeItem(bool val)
    {
        changeItem = val;
    }

    public void RegisterService()
    {
        ServiceLocator.RegisterService<IRoomdecorStore>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IRoomdecorStore>(this);
    }
}