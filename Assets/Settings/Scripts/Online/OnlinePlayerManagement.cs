using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePlayerManagement : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject buttons;
    [SerializeField] TMP_Text waitingMessage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            infoPanel.SetActive(false);
            waitingMessage.gameObject.SetActive(true);
            buttons.SetActive(false);
        }
    }
}
