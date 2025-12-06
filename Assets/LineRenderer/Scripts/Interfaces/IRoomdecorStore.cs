using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IRoomdecorStore : IGameService
{
    Canvas canvas {  get; }
    List<StoreItemData> itemSpriteData { get; }
    bool repositionItem { get; }
    bool changeItem { get; }
    void RepositionItem(bool val);
    void ChangeItemSprite(Image item, Sprite sprite);
    void ChangeItem(bool val);
}
