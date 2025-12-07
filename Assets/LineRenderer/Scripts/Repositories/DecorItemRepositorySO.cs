using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecorItems Repository", menuName = "MetaData/Repositories/Create DecorItems Repository")]
public class DecorItemRepositorySO : ScriptableObject
{
    [SerializeField] private List<DecorIteamMetaDataSO> _decorItems;

    public List<DecorIteamMetaDataSO> GetItemsByType(DecorItemType type) =>
        _decorItems.FindAll(x => x.ItemType == type);
    
    public DecorIteamMetaDataSO GetItem(DecorItemType type) =>
        _decorItems.Find(x => x.ItemType == type);
}
