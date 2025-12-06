using UnityEngine;

[CreateAssetMenu(fileName = "RoomDecorItem", menuName = "MetaData/RoomDecorItems/Create Item", order = 0)]
public class RoomDecorIteamMetaDataSO : ScriptableObject
{
    [field: SerializeField] public DecorItemName ItemName { get; private set; }
    [field: SerializeField] public DecorItemType ItemType { get; private set; }
    
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite ItemIcon { get; private set; }
    
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public CurrencyObject Price { get; private set; }
}
