using UnityEngine;

[CreateAssetMenu(fileName = "RoomDecorItem", menuName = "MetaData/RoomDecorItems/Create Item", order = 1)]
public class RoomDecorIteamMetaDataSO : ScriptableObject
{
    [field: SerializeField] public BedRoomItemName ItemName { get; private set; }
    [field: SerializeField] public BedRoomItemType ItemType { get; private set; }
    
    
}
