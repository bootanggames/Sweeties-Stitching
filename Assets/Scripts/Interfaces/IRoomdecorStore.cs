using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IRoomdecorStore : IGameService
{
    GameObject bedroom {  get; }        
    GameObject myItemsScreen {  get; }
    GameObject changeMyRoomScreen {  get; }        
    GameObject myRoomScreen {  get; }        
    Canvas canvas {  get; }
    bool repositionItem { get; }
    bool changeItem { get; }
    void RepositionItem(bool val);
    void ChangeItemSprite(Image item, Sprite sprite);
    void ChangeItem(bool val);
    void EnableDisableChangeRoomUiParent(bool val);
    void EnableDisableMItemsScreen(bool val);
    void EnableDisableMyRoomScreen(bool val);
    void MyRoomButton();
    void MyItemsButton(bool val);
    void StopSound();
    void OpenScreen();
    List<GameObject> roomScreenButtons {  get; }
}
