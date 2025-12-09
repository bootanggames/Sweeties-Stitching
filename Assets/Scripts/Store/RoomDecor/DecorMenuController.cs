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
    _bedsButton.onClick.AddListener(() => ShowInventory(DecorItemType.BED));
    _shelfButton.onClick.AddListener(() => ShowInventory(DecorItemType.SHELF));
    _machineButton.onClick.AddListener(() => ShowInventory(DecorItemType.SEWING_MACHINE));
    
    _ceilingButton.onClick.AddListener(() => ShowInventory(DecorItemType.ROOF));
    _wallsButton.onClick.AddListener(() => ShowInventory(DecorItemType.WALL));
    _floorsButton.onClick.AddListener(() => ShowInventory(DecorItemType.FLOOR));
  }

  void ShowInventory(DecorItemType itemType)
  {
    _decorItemsInventory.ShowByType(itemType);
  }
}
