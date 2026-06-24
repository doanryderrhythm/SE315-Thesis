using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject roomPrefab;
    GameObject[] allRooms = new GameObject[0];

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < allRooms.Length; i++)
        {
            if (allRooms[i] != null)
                Destroy(allRooms[i]);
        }

        allRooms = new GameObject[roomList.Count];

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].IsOpen && roomList[i].IsVisible && roomList[i].PlayerCount >= 1)
            {
                GameObject room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform);

                RoomElement roomElement = room.GetComponent<RoomElement>();
                roomElement.roomText.text = roomList[i].Name;

                allRooms[i] = room;
            }
        }
    }
}
