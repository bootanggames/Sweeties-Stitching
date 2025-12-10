using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
   //[SerializeField] private Button _decorButton;
   [SerializeField] private Image _decorImage;

   [field:SerializeField] public DecorItemType _decorItemType {  get; private set; }
   [SerializeField] private DecorItemRepositorySO _repository;

   private bool _canChange = false;
    UpdateRoom upgradeRoom;
   private void Start()
   {
        upgradeRoom = GetComponentInParent<UpdateRoom>();
        //_decorButton.onClick.AddListener(OnButtonPress);
    }

    private void OnEnable()
   {
        upgradeRoom = GetComponentInParent<UpdateRoom>();
      GameEvents.RoomDecorEvents.SetRoomDecorPermissionStatus.Register(OnSetRoomDecorPermissionStatus);
      GameEvents.RoomDecorEvents.DecorItemSelected.Register(OnDecorItemSelected);
   }

   private void OnDisable()
   {
      GameEvents.RoomDecorEvents.SetRoomDecorPermissionStatus.UnRegister(OnSetRoomDecorPermissionStatus);
      GameEvents.RoomDecorEvents.DecorItemSelected.UnRegister(OnDecorItemSelected);
   }

   private void OnSetRoomDecorPermissionStatus(bool status)
   {
      _canChange = status;
   }
    public void ChangeItemImage(Sprite itemSprite)
    {
        _decorImage.sprite = itemSprite;

    }
    private void OnDecorItemSelected(DecorItemName decorItemName, DecorItemType decorItemType)
   {
      Debug.Log($"OnDecorItemSelected {decorItemType}");
      if (decorItemType != _decorItemType)
         return;
        if (!decorItemType.Equals(DecorItemType.SHELF))
            _decorImage.sprite = _repository.GetItem(decorItemName).ItemSprite;
        else
            upgradeRoom.UpdateShelf(decorItemName);
        SaveItems(decorItemType, decorItemName.ToString());
    }

    public void SaveItems(DecorItemType itemType, string itemName)
    {
        foreach (DecorIteamMetaDataSO metaData in _repository.GetItemsByType(itemType))
        {
            PlayerPrefs.SetInt(metaData.ItemName.ToString(), 0);
        }
        PlayerPrefs.SetInt(itemName, 1);
    }
    private void OnButtonPress()
   {
      Debug.LogError("OnButtonPress");
      
      if (!_canChange)
         return;
      
      GameEvents.UIEvents.ShowDecorItemsInventory.Raise(_decorItemType);
   }
}
