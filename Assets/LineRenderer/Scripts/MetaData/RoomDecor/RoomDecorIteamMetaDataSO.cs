using UnityEngine;

[CreateAssetMenu(fileName = "RoomDecorItem", menuName = "MetaData/RoomDecorItems/Create Item", order = 0)]
public class RoomDecorIteamMetaDataSO : ScriptableObject
{
    [field: SerializeField] public BedRoomItemName ItemName { get; private set; }
    [field: SerializeField] public BedRoomItemType ItemType { get; private set; }
    
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public Sprite ItemIcon { get; private set; }
    
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public CurrencyObject Price { get; private set; }
}
