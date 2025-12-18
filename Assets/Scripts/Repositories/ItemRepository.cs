using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items Repository", menuName = "MetaData/Repositories/Create Items Repository")]
public class ItemRepository : ScriptableObject
{
    [SerializeField] private List<ItemsMetaData> _decorItems;

    public List<ItemsMetaData> GetItemsByType(ItemType type) =>
        _decorItems.FindAll(x => x.ItemType == type);
    
    public ItemsMetaData GetItem(ItemName itemName) =>
        _decorItems.Find(x => x.ItemName == itemName);
}
