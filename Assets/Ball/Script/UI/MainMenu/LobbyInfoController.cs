using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyInfoController : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameTxt;
    [SerializeField] private TMP_Text playerNameTxt;
    [SerializeField] private TMP_Text eloTxt;


    public void SetInfo(string roomName, string playerName, int elo)
    {
        roomNameTxt.text = roomName;
        playerNameTxt.text = playerName;
        eloTxt.text = elo.ToString();
    }
}
