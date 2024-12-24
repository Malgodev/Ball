using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoController : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameTxt;
    [SerializeField] private TMP_Text playerNameTxt;
    [SerializeField] private TMP_Text eloTxt;
    [SerializeField] private Button joinBtn;

    private string lobbyId = "";

    private void Start()
    {
        joinBtn.onClick.AddListener(() =>
        {
            Debug.Log(lobbyId);
            BallGameLobby.Instance.JoinLobbyById(lobbyId);
        });
    }

    public void SetInfo(string lobbyId, string roomName, string elo)
    {
        this.lobbyId = lobbyId;
        roomNameTxt.text = roomName;
        eloTxt.text = elo;
    }
}
