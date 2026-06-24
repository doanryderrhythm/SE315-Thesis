using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] CreateAndJoin createAndJoin;
    [SerializeField] GameObject roomPrefab;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform);

            RoomElement roomElement = room.GetComponent<RoomElement>();
            roomElement.roomText.text = roomList[i].Name;
        }
    }
}
