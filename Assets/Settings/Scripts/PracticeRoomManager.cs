using UnityEngine;
using Unity.Netcode;

public class PracticeRoomManager : MonoBehaviour
{
    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
        }
    }
}
