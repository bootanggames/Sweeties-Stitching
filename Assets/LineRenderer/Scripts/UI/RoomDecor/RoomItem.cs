using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
   [SerializeField] private Button _decorButton;
   [SerializeField] private Image _decorImage;

   [SerializeField] private DecorItemType _decorItemType;
   [SerializeField] private DecorItemRepositorySO _repository;

   private bool _canChange = false;

   private void Start()
   {
      _decorButton.onClick.AddListener(OnButtonPress);
   }

   private void OnEnable()
   {
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

   private void OnDecorItemSelected(DecorItemName decorItemName, DecorItemType decorItemType)
   {
      Debug.Log($"OnDecorItemSelected {decorItemType}");
      if (decorItemType != _decorItemType)
         return;

      _decorImage.sprite = _repository.GetItem(decorItemName).ItemSprite;
   }

   private void OnButtonPress()
   {
      Debug.LogError("OnButtonPress");
      
      if (!_canChange)
         return;
      
      GameEvents.UIEvents.ShowDecorItemsInventory.Raise(_decorItemType);
   }
}
