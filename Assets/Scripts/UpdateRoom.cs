using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateRoom : MonoBehaviour
{
    [SerializeField] private ItemRepository _repository;
    [field: SerializeField] public List<RoomItem> roomitem {  get; private set; }

    [field: SerializeField] public bool saveRoom {  get; private set; }
    [field: SerializeField]public List<GameObject> shelf {  get; private set; }
    public BedroomStates bedroomState;

    void OnEnable()
    {
        int val = PlayerPrefs.GetInt("SaveRoom");
        //if (val == 1)
        //    saveRoom = true;
        //else
        //    saveRoom = false;
        //if(saveRoom)
            UpdateChanges();
    }
   

    void UpdateChanges()
    {
        foreach (RoomItem item in roomitem)
        {
            foreach (ItemsMetaData metaData in _repository.GetItemsByType(item._decorItemType))
            {
                int state = PlayerPrefs.GetInt(metaData.ItemName.ToString());
                //if (state == 1)
                //    item.ChangeItemImage(_repository.GetItem(metaData.ItemName).ItemSprite);
                if (!item._decorItemType.Equals(ItemType.SHELF))
                {
                    if (state == 1)
                        item.ChangeItemImage(_repository.GetItem(metaData.ItemName).ItemSprite);
                }
                else
                {
                    if (state == 1)
                    {
                        UpdateShelf(metaData.ItemName);
                    }
                }
            }
        }
       
    }
    public void UpdateShelf(ItemName _itemName)
    {
        foreach (GameObject g in shelf)
        {
            Plushie_ShelfContainer _shelf = g.GetComponent<Plushie_ShelfContainer>();
            if (_shelf.itemName.Equals(_itemName))
            {
                foreach (GameObject s in shelf)
                {
                    s.SetActive(false);
                }
                g.SetActive(true);
                break;
            }

        }
    }
    public void SaveRoom(int val)
    {
        PlayerPrefs.SetInt("SaveRoom", val);
    }
    public void UpdateRoomState(string state)
    {
        switch (state)
        {
            case "home":
                bedroomState = BedroomStates.home;
                break;
            case "decor":
                bedroomState = BedroomStates.decor;
                break;
            case "move":
                bedroomState = BedroomStates.move;
                break;
        }
    }
}
