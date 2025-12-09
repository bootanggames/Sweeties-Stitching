using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateRoom : MonoBehaviour,IRoomUpgrade
{
    [SerializeField] private DecorItemRepositorySO _repository;
    [SerializeField] List<RoomItem> roomitem;

    [field: SerializeField] public bool saveRoom {  get; private set; }

    void OnEnable()
    {
        RegisterService();
        int val = PlayerPrefs.GetInt("SaveRoom");
        //if (val == 1)
        //    saveRoom = true;
        //else
        //    saveRoom = false;
        //if(saveRoom)
            UpdateChanges();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<IRoomUpgrade>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<IRoomUpgrade>(this);
    }

    void UpdateChanges()
    {
        foreach(RoomItem item in roomitem)
        {
            foreach (DecorIteamMetaDataSO metaData in _repository.GetItemsByType(item._decorItemType))
            {
                int state = PlayerPrefs.GetInt(metaData.ItemName.ToString());
                if(state == 1)
                    item.ChangeItemImage( _repository.GetItem(metaData.ItemName).ItemSprite);
            }
        }
       
    }
    public void SaveRoom(int val)
    {
        PlayerPrefs.SetInt("SaveRoom", val);
    }
}
