using System;
using UnityEngine;
using UnityEngine.UI;

public class DecorMenuController : MonoBehaviour
{
  [SerializeField] private DecorItemsInventory _decorItemsInventory;
  
  [SerializeField] private Button _bedsButton;
  [SerializeField] private Button _shelfButton;
  [SerializeField] private Button _machineButton;

  [SerializeField] private Button _ceilingButton;
  [SerializeField] private Button _wallsButton;
  [SerializeField] private Button _floorsButton;

  private void Start()
  {
    _bedsButton.onClick.AddListener(() => ShowInventory(ItemType.BED));
    _shelfButton.onClick.AddListener(() => ShowInventory(ItemType.SHELF));
    _machineButton.onClick.AddListener(() => ShowInventory(ItemType.SEWING_MACHINE));
    
    _ceilingButton.onClick.AddListener(() => ShowInventory(ItemType.ROOF));
    _wallsButton.onClick.AddListener(() => ShowInventory(ItemType.WALL));
    _floorsButton.onClick.AddListener(() => ShowInventory(ItemType.FLOOR));
  }

  void ShowInventory(ItemType itemType)
  {
    _decorItemsInventory.ShowByType(itemType);
  }
}
