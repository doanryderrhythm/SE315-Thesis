using TMPro;
using UnityEngine;

public class RoomElement : MonoBehaviour
{
    public TMP_Text roomText;

    public void ChooseRoom()
    {
        CreateAndJoin CAJ = GameObject.FindAnyObjectByType<CreateAndJoin>();
        CAJ.JoinRoom(roomText.text);
    }
}
