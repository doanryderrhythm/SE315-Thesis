using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject roomPrefab;
    GameObject[] allRooms = new GameObject[0];

    private Dictionary<string, GameObject> roomEntries = new Dictionary<string, GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                if (roomEntries.TryGetValue(info.Name, out GameObject room))
                {
                    Destroy(room);
                    roomEntries.Remove(info.Name);
                }

                continue;
            }

            if (!roomEntries.ContainsKey(info.Name))
            {
                GameObject room = Instantiate(roomPrefab, transform);

                RoomElement element = room.GetComponent<RoomElement>();
                element.roomText.text = info.Name;

                roomEntries.Add(info.Name, room);
            }
            else
            {
                RoomElement element =
                    roomEntries[info.Name].GetComponent<RoomElement>();

                element.roomText.text = info.Name;
            }
        }
    }
}
