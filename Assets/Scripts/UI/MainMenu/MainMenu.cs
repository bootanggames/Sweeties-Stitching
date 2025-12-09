using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _myRoombutton;
    
    void Start()
    {
        _myRoombutton.onClick.AddListener(OnMyRoomClicked);
    }

    private void OnMyRoomClicked()
    {
        GameEvents.RoomDecorEvents.SetRoomDecorPermissionStatus.Raise(true);
    }
}
