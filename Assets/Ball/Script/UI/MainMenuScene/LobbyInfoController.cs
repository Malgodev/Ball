using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoController : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameTxt;
    [SerializeField] private TMP_Text playerNameTxt;
    [SerializeField] private TMP_Text eloTxt;
    [SerializeField] private Button joinBtn;

    private string lobbyCode = "";
    private string lobbyId = "";

    private void Start()
    {
        joinBtn.onClick.AddListener(() =>
        {
            Debug.Log(lobbyCode + " " + lobbyId);
            BallGameLobby.Instance.JoinLobbyById(lobbyId);
        });
    }

    public void SetInfo(string lobbyId, string lobbyCode, string roomName, int elo)
    {
        this.lobbyId = lobbyId;
        this.lobbyCode = lobbyCode;
        roomNameTxt.text = roomName;
        eloTxt.text = elo.ToString();
    }
}
