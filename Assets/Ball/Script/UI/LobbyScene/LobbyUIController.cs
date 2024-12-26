using Malgo.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LobbyUIController : MonoBehaviour
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
        readyBtn.onClick.AddListener(() =>    
        {
            LobbyMultiplayerManager.Instance.SetLocalPlayerReady();
        });

    }

    public void SetLobbyInfo()
    {
        roomName.text = BallGameLobby.Instance.LobbyName;

        List<PlayerInfo> playerList = BallGameLobby.Instance.GetPlayerInfoList();

        foreach (PlayerInfo playerInfo in playerList)
        {
            Debug.Log(playerInfo);
        }

        playerOneInfoTxt.text = playerList[0].PlayerName;
        playerTwoInfoTxt.text = playerList[1].PlayerName;
    }
    

    public void SetPlayerReadyUI(Dictionary<ulong, bool> playerReadyDict)
    {
        foreach (ulong clientId in playerReadyDict.Keys)
        {
            if (clientId == 0)
            {
                playerOneIsReadyTxt.text = playerReadyDict[clientId] ? "Ready" : "Not ready";
            }
            else
            {
                playerTwoIsReadyTxt.text = playerReadyDict[clientId] ? "Ready" : "Not ready";
            }
        }
    }
}
