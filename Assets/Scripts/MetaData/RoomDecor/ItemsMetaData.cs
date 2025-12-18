using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "MetaData/Items/Create Item", order = 0)]
public class ItemsMetaData : ScriptableObject
{
    [field: SerializeField] public ItemName ItemName { get; private set; }
    [field: SerializeField] public ItemType ItemType { get; private set; }
    
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite ItemIcon { get; private set; }
    
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public CurrencyObject Price { get; private set; }
}
