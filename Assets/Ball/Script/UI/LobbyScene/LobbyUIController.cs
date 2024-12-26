using Malgo.UI;
using TMPro;
using Unity.Netcode;
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
        roomName.text = BallPlayerInfo.Instance.PlayerName;

        readyBtn.onClick.AddListener(() =>
        {
            LobbyMultiplayerManager.Instance.SetLocalPlayerReadyServerRpc(BallPlayerInfo.Instance.PlayerName);
        });
    }

    public void SetPlayerInfoUI(LobbyPlayerInfo playerOne, LobbyPlayerInfo playerTwo)
    {
        playerOneInfoTxt.text = playerOne.PlayerName;
        playerOneIsReadyTxt.text = playerOne.IsPlayerReady ? "Ready" : "Not ready";

        playerTwoInfoTxt.text = playerTwo.PlayerName;
        playerTwoIsReadyTxt.text = playerTwo.IsPlayerReady ? "Ready" : "Not ready";
    }
}
