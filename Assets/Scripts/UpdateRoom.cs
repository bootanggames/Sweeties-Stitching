using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateRoom : MonoBehaviour
{
    [SerializeField] private DecorItemRepositorySO _repository;
    [SerializeField] List<RoomItem> roomitem;

    [field: SerializeField] public bool saveRoom {  get; private set; }
    [SerializeField] List<GameObject> shelf;
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
            foreach (DecorIteamMetaDataSO metaData in _repository.GetItemsByType(item._decorItemType))
            {
                int state = PlayerPrefs.GetInt(metaData.ItemName.ToString());
                if (!item._decorItemType.Equals(DecorItemType.SHELF))
                {
                    if (state == 1)
                        item.ChangeItemImage(_repository.GetItem(metaData.ItemName).ItemSprite);
                }
                else
                {
                    if(state == 1)
                    {
                        UpdateShelf(metaData.ItemName);
                    }
                }
            }
        }
       
    }
    public void UpdateShelf(DecorItemName _itemName)
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
}
