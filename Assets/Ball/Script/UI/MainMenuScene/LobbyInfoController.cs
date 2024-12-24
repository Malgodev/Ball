using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyInfoController : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameTxt;
    [SerializeField] private TMP_Text playerNameTxt;
    [SerializeField] private TMP_Text eloTxt;

    public void SetInfo(string roomName, int elo)
    {
        roomNameTxt.text = roomName;
        eloTxt.text = elo.ToString();
    }
}
