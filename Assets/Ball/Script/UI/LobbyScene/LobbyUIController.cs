using Malgo.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : NetworkBehaviour
{
    [Header("Lobby Info")]
    // TODO Change the textfield to inputfield => user can customize room name.
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private ToggleSwitch isPrivate;


    [Header("Button")]

    [SerializeField] private Button readyBtn;
    [SerializeField] private Button returnBtn;

    [Header("Player panel")]

    [SerializeField] private TMP_Text playerOneInfoTxt;
    [SerializeField] private TMP_Text playerOneIsReadyTxt;

    [SerializeField] private TMP_Text playerTwoInfoTxt;
    [SerializeField] private TMP_Text playerTwoIsReadyTxt;

    private void Start()
    {
        roomName.text = BallPlayerInfo.Instance.PlayerName;
        Debug.Log(roomName.text + " " + BallPlayerInfo.Instance.PlayerName);
    }
}
